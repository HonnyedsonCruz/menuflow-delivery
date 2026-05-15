const API_BASE_URL = "http://localhost:5094/api";

async function apiGet(endpoint, token = null) {
  const headers = {};

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    headers
  });

  return await response.json();
}

async function apiPost(endpoint, body, token = null) {
  const headers = {
    "Content-Type": "application/json"
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: "POST",
    headers,
    body: JSON.stringify(body)
  });

  return await response.json();
}

async function apiPut(endpoint, body, token = null) {
  const headers = {
    "Content-Type": "application/json"
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: "PUT",
    headers,
    body: JSON.stringify(body)
  });

  return await response.json();
}

async function apiPatch(endpoint, token = null) {
  const headers = {};

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: "PATCH",
    headers
  });

  return await response.json();
}

async function apiDelete(endpoint, token = null) {
  const headers = {};

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: "DELETE",
    headers
  });

  return await response.json();
}