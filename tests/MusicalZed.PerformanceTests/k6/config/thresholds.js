/**
 * Musical Zed - k6 Performance Thresholds
 * Limites de SLA para todos os testes
 */
export const defaultThresholds = {
  http_req_duration: ['p(95)<2000', 'p(99)<5000'],
  http_req_failed: ['rate<0.01'],
  http_reqs: ['rate>5'],
};

export const strictThresholds = {
  http_req_duration: ['p(95)<500', 'p(99)<1000'],
  http_req_failed: ['rate<0.005'],
};
