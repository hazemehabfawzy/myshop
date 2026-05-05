import axiosClient from './axiosClient';

const productService = {
  getAll: async (params) => {
    const response = await axiosClient.get('/products', { params });
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosClient.get(`/products/${id}`);
    return response.data;
  },
  search: async (query) => {
    const response = await axiosClient.get('/products/search', { params: { q: query } });
    return response.data;
  },
  create: async (data) => {
    const response = await axiosClient.post('/products', data);
    return response.data;
  },
  update: async (id, data) => {
    const response = await axiosClient.put(`/products/${id}`, data);
    return response.data;
  },
  remove: async (id) => {
    const response = await axiosClient.delete(`/products/${id}`);
    return response.data;
  },
};

export default productService;
