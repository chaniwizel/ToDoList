import axios from 'axios';


axios.defaults.baseURL = "http://localhost:5000";

export default {

  getTasks: async () => {
    const result = await axios.get(`/item`)   
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    const result = await axios.post(`/item`, { name });
    return result.data;
    
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    const result = await axios.put(`/item/${id}`, { isComplete });
    return result.data;
  },

  deleteTask:async(id)=>{
    console.log('deleteTask')
    await axios.delete(`/item/${id}`);
  }
};
