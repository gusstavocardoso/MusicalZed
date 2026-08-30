using System.Reflection;
using Microsoft.OpenApi.Models;
using MusicalZed.Application;
using MusicalZed.Infrastructure;
using MusicalZed.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Musical Zed API",
        Version = "v1",
        Description = """
            API REST da loja virtual **Musical Zed** — sua loja de instrumentos musicais.
            
            ## Recursos disponíveis
            - **Produtos**: listagem, busca, filtro por categoria e detalhes
            - **Categorias**: listagem e detalhes
            - **Carrinho**: gerenciamento de itens por sessão
            - **Pedidos**: criação e consulta de pedidos
            
            ## Regras de negócio
            - Frete **grátis** para pedidos acima de R$ 500,00
            - Frete de **R$ 29,90** para pedidos abaixo desse valor
            - O carrinho é identificado por `sessionId` (UUID)
            """,
        Contact = new OpenApiContact
        {
            Name = "Musical Zed",
            Email = "contato@musicalzed.com.br",
            Url = new Uri("https://musicalzed.com.br")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.EnableAnnotations();

    // Exemplo de tag ordering
    options.TagActionsBy(api => [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "API"]);
    options.DocInclusionPredicate((_, _) => true);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicalZedDbContext>();
    await DataSeeder.SeedAsync(db);
}

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Musical Zed API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Musical Zed API";
    c.DefaultModelsExpandDepth(2);
    c.DefaultModelExpandDepth(2);
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
    c.InjectStylesheet("/swagger-ui/custom.css");
});

app.UseStaticFiles();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Redirect raiz para swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();

public partial class Program { }
