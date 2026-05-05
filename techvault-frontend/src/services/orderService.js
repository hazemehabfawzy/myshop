import axiosClient from './axiosClient';

const orderService = {
  getMyOrders: async () => {
    const response = await axiosClient.get('/orders/my');
    return response.data;
  },
  getAll: async () => {
    const response = await axiosClient.get('/orders');
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosClient.get(`/orders/${id}`);
    return response.data;
  },
  create: async (data) => {
    const response = await axiosClient.post('/orders', data);
    return response.data;
  },
  updateStatus: async (id, status) => {
    const response = await axiosClient.put(`/orders/${id}/status`, { status });
    return response.data;
  },
  cancel: async (id) => {
    const response = await axiosClient.put(`/orders/${id}/cancel`);
    return response.data;
  },
};

export default orderService;
