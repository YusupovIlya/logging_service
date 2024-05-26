import axios from 'axios';

const API_BASE_URL = 'http://localhost:7087/api/cl';


const getData = async (url, queryParams) => {
  try {
    const response = await axios.post(url, queryParams);
    return response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
    throw error;
  } 
}

export const getFilteredEvents = async (queryParams) => 
  getData(`${API_BASE_URL}/stream-data/filter?timestampSortDir=desc`, queryParams);

export const getAggregatedEvents = async (queryParams) =>
  getData(`${API_BASE_URL}/stream-data/aggregate`, queryParams);

export const getFileEvents = async (queryParams, fileType) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/stream-data/filter/file?type=${fileType}&timestampSortDir=desc`, queryParams, {responseType: 'blob'}); 
    return response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
    throw error;
  } 
}