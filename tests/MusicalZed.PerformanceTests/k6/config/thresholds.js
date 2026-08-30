/**
 * Musical Zed - k6 Performance Thresholds
 * Limites de SLA para todos os testes
 *
 * Nota: http_reqs (rate) foi removido dos thresholds padrão porque
 * depende do número de VUs e do sleep configurado em cada cenário,
 * não sendo um indicador de qualidade de serviço.
 */
export const defaultThresholds = {
  // 95% das requisições devem completar em menos de 2s
  http_req_duration: ['p(95)<2000', 'p(99)<5000'],
  // Taxa de erros deve ser menor que 1%
  http_req_failed: ['rate<0.01'],
};

export const strictThresholds = {
  http_req_duration: ['p(95)<500', 'p(99)<1000'],
  http_req_failed: ['rate<0.005'],
};
