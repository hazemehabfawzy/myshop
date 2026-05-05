import axiosClient from './axiosClient';

const serviceRequestService = {
  getMy: async () => {
    const response = await axiosClient.get('/service-requests/my');
    return response.data;
  },
  getAll: async () => {
    const response = await axiosClient.get('/service-requests');
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosClient.get(`/service-requests/${id}`);
    return response.data;
  },
  create: async (data) => {
    const response = await axiosClient.post('/service-requests', data);
    return response.data;
  },
  updateStatus: async (id, status) => {
    const response = await axiosClient.put(`/service-requests/${id}/status`, { status });
    return response.data;
  },
};

export default serviceRequestService;
