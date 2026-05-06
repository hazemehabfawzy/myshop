import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import './App.css';

// Pages
import HomePage from './pages/HomePage';
import ProductsPage from './pages/ProductsPage';
import ProductDetailPage from './pages/ProductDetailPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import OrdersPage from './pages/OrdersPage';
import CreateOrderPage from './pages/CreateOrderPage';
import ServiceRequestsPage from './pages/ServiceRequestsPage';
import CreateServiceRequestPage from './pages/CreateServiceRequestPage';
import AddProductPage from './pages/AddProductPage';
import EditProductPage from './pages/EditProductPage';
import StockManagementPage from './pages/StockManagementPage';
import NotFoundPage from './pages/NotFoundPage';

function App() {
  return (
    <>
      <Navbar />
      <main className="main-content">
        <Routes>
          {/* Public Routes */}
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/products/:id" element={<ProductDetailPage />} />

          {/* Protected Routes (Customer/Admin) */}
          <Route 
            path="/orders" 
            element={
              <ProtectedRoute>
                <OrdersPage />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/orders/new" 
            element={
              <ProtectedRoute>
                <CreateOrderPage />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/service-requests" 
            element={
              <ProtectedRoute>
                <ServiceRequestsPage />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/service-requests/new" 
            element={
              <ProtectedRoute>
                <CreateServiceRequestPage />
              </ProtectedRoute>
            } 
          />

          {/* Admin Only Routes */}
          <Route 
            path="/admin/products/new" 
            element={
              <ProtectedRoute requiredRole="Admin">
                <AddProductPage />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/admin/products/:id/edit" 
            element={
              <ProtectedRoute requiredRole="Admin">
                <EditProductPage />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/admin/stock" 
            element={
              <ProtectedRoute requiredRole="Admin">
                <StockManagementPage />
              </ProtectedRoute>
            } 
          />

          {/* 404 Route */}
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>
    </>
  );
}

export default App;
