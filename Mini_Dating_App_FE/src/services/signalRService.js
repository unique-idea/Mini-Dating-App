import * as signalR from '@microsoft/signalr'

// const URL = import.meta.env.VITE_API_BASE_URL?.replace('/api/v1', '')
//For some problem with dockerfile it can read the evn, and not have enough time
//for fix, so i force to using this
const URL = "http://localhost:5016";
let connection = null

const buildConnection = () => {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${URL}/hubs`, {
        accessTokenFactory: () => localStorage.getItem('token')
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()
  }
  
export const startConnection = async () => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${URL}/hubs`, {
      accessTokenFactory: () => localStorage.getItem('token')
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build()

  try {
    await connection.start()
    console.log('✅ SignalR connected')
  } catch (err) {
    console.error('❌ SignalR connection failed:', err)
  }
}

export const joinUserGroup = async (userId) => {
  if (connection) {
    await connection.invoke('JoinUserGroup', userId)
    console.log('👥 Joined group:', userId)
  }
}

export const stopConnection = async () => {
  if (connection) {
    await connection.stop()
    connection = null
    console.log('🔌 SignalR disconnected')
  }
}
export const onMatchCreated = (callback) => {
    if (!connection) buildConnection()
    connection.off('matchcreated') 
    connection.on('matchcreated', callback)
  }
  
  export const onMatchUpdated = (callback) => {
    if (!connection) buildConnection()
    connection.off('matchupdated')
    connection.on('matchupdated', callback)
  }
  
  export const offMatchCreated = () => {
    if (connection) connection.off('matchcreated')
  }
  
  export const offMatchUpdated = () => {
    if (connection) connection.off('matchupdated')
  }