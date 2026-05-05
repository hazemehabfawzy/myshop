import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import LoadingSpinner from './LoadingSpinner';

const ProtectedRoute = ({ children, requiredRole }) => {
  const { isAuthenticated, user, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return <LoadingSpinner message="Checking authentication..." />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (requiredRole && user?.role !== requiredRole) {
    return (
      <div className="container" style={{ textAlign: 'center', marginTop: '5rem' }}>
        <h1 style={{ color: 'var(--danger)' }}>Access Denied</h1>
        <p>You do not have permission to view this page.</p>
        <button className="btn btn-primary" onClick={() => window.history.back()} style={{ marginTop: '1rem' }}>
          Go Back
        </button>
      </div>
    );
  }

  return children;
};

export default ProtectedRoute;
