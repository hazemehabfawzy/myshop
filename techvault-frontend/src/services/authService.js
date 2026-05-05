import axiosClient from './axiosClient';

const authService = {
  login: async (username, password) => {
    const response = await axiosClient.post('/auth/login', { username, password });
    return response.data;
  },
  register: async (data) => {
    const response = await axiosClient.post('/auth/register', data);
    return response.data;
  },
};

export default authService;
