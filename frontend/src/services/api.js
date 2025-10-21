import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5212/api';

const api = axios.create({
  baseURL: API_BASE_URL,
});

export const getAvailability = async (tentType, startDate, endDate) => {
  try {
    const response = await api.get('/bookings/availability', {
      params: { tentType, startDate, endDate },
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching availability:', error);
    throw error;
  }
};

export const createBooking = async (bookingData) => {
  try {
    const response = await api.post('/bookings', bookingData);
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getDeliveryFee = async (address) => {
  try {
    const response = await api.post('/bookings/delivery-fee', address);
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getQuote = async (quoteRequest) => {
  try {
    const response = await api.post('/bookings/quote', quoteRequest);
    return response.data;
  } catch (error) {
    console.error('Error fetching quote:', error);
    throw error;
  }
};

export const login = async (email) => {
  try {
    const response = await api.post('/auth/login', { email });
    return response.data;
  } catch (error) {
    throw error;
  }
};
