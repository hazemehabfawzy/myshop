import React, { createContext, useContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [loading, setLoading] = useState(true);

  const login = (newToken) => {
    if (!newToken) return;
    
    try {
      localStorage.setItem('token', newToken);
      setToken(newToken);
      const decoded = jwtDecode(newToken);
      console.log('Decoded Token:', decoded);
      
      const userData = {
        id: decoded.UserId || decoded.sub || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        username: decoded.Username || decoded.name || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        email: decoded.Email || decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        role: decoded.Role || decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
      };
      
      setUser(userData);
      setIsAuthenticated(true);
      setIsAdmin(userData.role === 'Admin');
      localStorage.setItem('user', JSON.stringify(userData));
    } catch (err) {
      console.error('Login error:', err);
      logout();
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
    setIsAuthenticated(false);
    setIsAdmin(false);
  };

  useEffect(() => {
    if (token) {
      try {
        const decoded = jwtDecode(token);
        const currentTime = Date.now() / 1000;
        
        if (decoded.exp < currentTime) {
          logout();
        } else {
          const userData = JSON.parse(localStorage.getItem('user'));
          if (userData) {
            setUser(userData);
            setIsAuthenticated(true);
            setIsAdmin(userData.role === 'Admin');
          } else {
            // Re-decode if localStorage user is missing
            login(token);
          }
        }
      } catch (err) {
        logout();
      }
    }
    setLoading(false);
  }, [token]);

  return (
    <AuthContext.Provider value={{ user, token, isAuthenticated, isAdmin, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
