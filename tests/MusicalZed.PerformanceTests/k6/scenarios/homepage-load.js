/**
 * Musical Zed - Teste de Carga: Home Page
 * Smoke → Load → Stress
 */
import http from 'k6/http';
import { group } from 'k6';
import { API_BASE, checkResponse, jsonHeaders, randomSleep } from '../utils/helpers.js';
import { defaultThresholds } from '../config/thresholds.js';

export const options = {
  scenarios: {
    smoke: {
      executor: 'constant-vus', vus: 1, duration: '10s',
      tags: { scenario: 'smoke' },
    },
    load: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '15s', target: 20 },
        { duration: '30s', target: 20 },
        { duration: '15s', target: 0 },
      ],
      startTime: '15s',
      tags: { scenario: 'load' },
    },
    stress: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '10s', target: 60 },
        { duration: '20s', target: 60 },
        { duration: '10s', target: 0 },
      ],
      startTime: '75s',
      tags: { scenario: 'stress' },
    },
  },
  thresholds: defaultThresholds,
};

export default function () {
  group('Categorias', () => {
    const r = http.get(`${API_BASE}/api/categories`, { headers: jsonHeaders });
    checkResponse(r, 'GET /api/categories');
  });

  randomSleep(0.3, 0.7);

  group('Produtos em Destaque', () => {
    const r = http.get(`${API_BASE}/api/products/featured`, { headers: jsonHeaders });
    checkResponse(r, 'GET /api/products/featured');
  });

  randomSleep(1, 2);
}
