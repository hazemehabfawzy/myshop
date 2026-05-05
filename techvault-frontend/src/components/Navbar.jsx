import React from 'react';
import { NavLink, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Navbar.css';

const Navbar = () => {
  const { isAuthenticated, isAdmin, logout, user } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="container navbar-content">
        <Link to="/" className="navbar-logo">
          Tech<span>Vault</span>
        </Link>

        <div className="navbar-links">
          <NavLink to="/" end className={({ isActive }) => isActive ? 'active' : ''}>Home</NavLink>
          <NavLink to="/products" className={({ isActive }) => isActive ? 'active' : ''}>Products</NavLink>

          {isAuthenticated && (
            <>
              <NavLink to="/orders" className={({ isActive }) => isActive ? 'active' : ''}>My Orders</NavLink>
              <NavLink to="/service-requests" className={({ isActive }) => isActive ? 'active' : ''}>Service Requests</NavLink>
              
              {isAdmin && (
                <>
                  <NavLink to="/admin/products/new" className={({ isActive }) => isActive ? 'active' : ''}>+ Add Product</NavLink>
                  <NavLink to="/orders" className={({ isActive }) => isActive ? 'active' : ''}>All Orders</NavLink>
                </>
              )}
            </>
          )}
        </div>

        <div className="navbar-auth">
          {isAuthenticated ? (
            <div className="user-menu">
              <span className="username">Hi, {user?.username}</span>
              <button onClick={handleLogout} className="btn-logout">Logout</button>
            </div>
          ) : (
            <>
              <Link to="/login" className="btn-login">Login</Link>
              <Link to="/register" className="btn-register">Register</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
