import api from './api';

export const login = async (pin) => {
  const response = await api.post('/api/auth/login', { pin });
  if (response.data.token) {
    localStorage.setItem('jwt_token', response.data.token);
    localStorage.setItem('refresh_token', response.data.refreshToken);
    return response.data;
  }
  throw new Error('Login failed');
};

export const logout = () => {
  localStorage.removeItem('jwt_token');
  localStorage.removeItem('refresh_token');
};

export const isAuthenticated = () => {
  return !!localStorage.getItem('jwt_token');
};

export const getToken = () => {
  return localStorage.getItem('jwt_token');
};

