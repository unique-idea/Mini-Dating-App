import axios from 'axios'

const URL = import.meta.env.VITE_API_BASE_URL;
// const URL = "http://localhost:5016/api/v1";


const axiosClient = axios.create({
  baseURL: URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

axiosClient.interceptors.request.use((config) => {
  const authToken = localStorage.getItem('token')
  if (authToken) {
    config.headers.Authorization = `Bearer ${authToken}`
  }
  return config
})

export default axiosClient