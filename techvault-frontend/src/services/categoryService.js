import axiosClient from './axiosClient';

const categoryService = {
  getAll: async () => {
    const response = await axiosClient.get('/categories');
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosClient.get(`/categories/${id}`);
    return response.data;
  },
};

export default categoryService;
