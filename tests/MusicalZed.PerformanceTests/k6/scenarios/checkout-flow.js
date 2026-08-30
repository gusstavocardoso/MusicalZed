/**
 * Musical Zed - Teste de Performance: Checkout Completo
 * Produto → Carrinho → Pedido (fluxo end-to-end via API)
 */
import http from 'k6/http';
import { group, check } from 'k6';
import { API_BASE, checkoutSuccessRate, totalOrders, generateSessionId, jsonHeaders, randomSleep } from '../utils/helpers.js';

export const options = {
  scenarios: {
    checkout_flow: {
      executor: 'constant-vus', vus: 5, duration: '60s',
    },
  },
  thresholds: {
    http_req_duration:    ['p(95)<3000'],
    http_req_failed:      ['rate<0.02'],
    checkout_success_rate: ['rate>0.90'],
  },
};

const names    = ['Ana Silva', 'Carlos Souza', 'Maria Costa', 'Pedro Lima', 'Julia Santos'];
const states   = ['SP', 'RJ', 'MG', 'RS', 'PR'];
const payments = ['PIX', 'Cartão de Crédito', 'Boleto Bancário'];

export default function () {
  const sessionId = generateSessionId();
  let productId = null;

  group('1. Buscar Produto', () => {
    const r = http.get(`${API_BASE}/api/products`, { headers: jsonHeaders });
    if (r.status === 200) {
      const prods = JSON.parse(r.body);
      if (prods.length > 0) productId = prods[0].id;
    }
  });

  if (!productId) { checkoutSuccessRate.add(false); return; }

  randomSleep(1, 2);

  group('2. Adicionar ao Carrinho', () => {
    http.post(
      `${API_BASE}/api/carts/${sessionId}/items`,
      JSON.stringify({ productId, quantity: 1 }),
      { headers: jsonHeaders }
    );
  });

  randomSleep(2, 4);

  group('3. Finalizar Pedido', () => {
    const idx = Math.floor(Math.random() * names.length);
    const r = http.post(
      `${API_BASE}/api/orders`,
      JSON.stringify({
        customerName:  names[idx],
        email:         `perf${Date.now()}@teste.com`,
        phone:         '(11) 91234-5678',
        address:       'Av. Performance, 100',
        city:          'São Paulo',
        state:         states[idx % states.length],
        zipCode:       '01310-100',
        paymentMethod: payments[Math.floor(Math.random() * payments.length)],
        sessionId,
        notes:         '',
      }),
      { headers: jsonHeaders }
    );

    const success = check(r, {
      'order created 201': (r) => r.status === 201,
      'order has id':      (r) => r.status === 201 && JSON.parse(r.body).id > 0,
    });

    checkoutSuccessRate.add(success);
    if (success) totalOrders.add(1);
  });

  randomSleep(1, 2);
}
