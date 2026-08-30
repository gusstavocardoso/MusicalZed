/**
 * Musical Zed - k6 Helper Functions
 */
import { check, sleep } from 'k6';
import { Trend, Rate, Counter } from 'k6/metrics';

export const API_BASE = __ENV.API_URL || 'http://localhost:5000';
export const WEB_BASE = __ENV.WEB_URL || 'http://localhost:5002';

export const productLoadTime    = new Trend('product_load_time');
export const cartOperationTime  = new Trend('cart_operation_time');
export const checkoutSuccessRate = new Rate('checkout_success_rate');
export const totalOrders        = new Counter('total_orders_created');

export const jsonHeaders = {
  'Content-Type': 'application/json',
  'Accept': 'application/json',
};

export function checkResponse(response, name, expectedStatus = 200) {
  return check(response, {
    [`${name} - status ${expectedStatus}`]: (r) => r.status === expectedStatus,
    [`${name} - tem corpo`]:               (r) => r.body && r.body.length > 0,
    [`${name} - tempo < 2s`]:              (r) => r.timings.duration < 2000,
  });
}

export function generateSessionId() {
  return `perf-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

export function randomSleep(min = 0.5, max = 2) {
  sleep(min + Math.random() * (max - min));
}
