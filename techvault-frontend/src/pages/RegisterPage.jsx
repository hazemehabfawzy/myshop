import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import authService from '../services/authService';

const RegisterPage = () => {
  const [formData, setFormData] = useState({
    username: '',
    fullName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [fieldErrors, setFieldErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear field error when user types
    if (fieldErrors[name]) {
      setFieldErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const validate = () => {
    const errs = {};
    if (!formData.username.trim()) errs.username = 'Username is required';
    else if (formData.username.length < 3) errs.username = 'Username must be at least 3 characters';
    
    if (!formData.fullName.trim()) errs.fullName = 'Full name is required';
    if (!formData.email.trim()) errs.email = 'Email is required';
    else if (!/\S+@\S+\.\S+/.test(formData.email)) errs.email = 'Email is invalid';
    
    if (!formData.password) errs.password = 'Password is required';
    else if (formData.password.length < 6) errs.password = 'Password must be at least 6 characters';
    
    if (formData.password !== formData.confirmPassword) {
      errs.confirmPassword = 'Passwords do not match';
    }
    
    return errs;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length > 0) {
      setFieldErrors(errs);
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await authService.register({
        username: formData.username,
        fullName: formData.fullName,
        email: formData.email,
        password: formData.password
      });
      login(response.token || response.Token);
      navigate('/');
    } catch (err) {
      if (err.response?.data?.errors) {
        setFieldErrors(err.response.data.errors);
      } else {
        setError(err.response?.data?.message || 'Registration failed');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container" style={{ display: 'flex', justifyContent: 'center', padding: '5rem 0' }}>
      <div className="card" style={{ width: '100%', maxWidth: '450px', backgroundColor: 'var(--card-bg)', padding: '2rem', borderRadius: 'var(--radius)', border: '1px solid var(--border)' }}>
        <h2 style={{ textAlign: 'center', marginBottom: '2rem' }}>Create Account</h2>
        
        {error && <div className="error-text" style={{ marginBottom: '1rem', textAlign: 'center', padding: '0.5rem', backgroundColor: 'rgba(239, 68, 68, 0.1)', borderRadius: '4px' }}>{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Username</label>
            <input 
              name="username"
              type="text" 
              className={`form-control ${fieldErrors.username ? 'error' : ''}`}
              value={formData.username}
              onChange={handleChange}
              placeholder="hazemdoe"
            />
            {fieldErrors.username && <p className="error-text">{fieldErrors.username}</p>}
          </div>

          <div className="form-group">
            <label>Full Name</label>
            <input 
              name="fullName"
              type="text" 
              className={`form-control ${fieldErrors.fullName ? 'error' : ''}`}
              value={formData.fullName}
              onChange={handleChange}
              placeholder="Hazem Fawzy"
            />
            {fieldErrors.fullName && <p className="error-text">{fieldErrors.fullName}</p>}
          </div>

          <div className="form-group">
            <label>Email Address</label>
            <input 
              name="email"
              type="email" 
              className={`form-control ${fieldErrors.email ? 'error' : ''}`}
              value={formData.email}
              onChange={handleChange}
              placeholder="name@example.com"
            />
            {fieldErrors.email && <p className="error-text">{fieldErrors.email}</p>}
          </div>
          
          <div className="form-group">
            <label>Password</label>
            <input 
              name="password"
              type="password" 
              className={`form-control ${fieldErrors.password ? 'error' : ''}`}
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
            />
            {fieldErrors.password && <p className="error-text">{fieldErrors.password}</p>}
          </div>

          <div className="form-group">
            <label>Confirm Password</label>
            <input 
              name="confirmPassword"
              type="password" 
              className={`form-control ${fieldErrors.confirmPassword ? 'error' : ''}`}
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="••••••••"
            />
            {fieldErrors.confirmPassword && <p className="error-text">{fieldErrors.confirmPassword}</p>}
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? 'Creating Account...' : 'Register'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: '1.5rem', fontSize: '0.875rem', color: 'var(--text-muted)' }}>
          Already have an account? <Link to="/login" style={{ color: 'var(--accent)', fontWeight: '600' }}>Login</Link>
        </p>
      </div>
    </div>
  );
};

export default RegisterPage;
