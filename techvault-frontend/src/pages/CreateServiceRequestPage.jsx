import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import serviceRequestService from '../services/serviceRequestService';

const CreateServiceRequestPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [formData, setFormData] = useState({
    deviceType: '',
    brand: '',
    model: '',
    issueDescription: '',
    serviceType: ''
  });

  const [serviceOptions, setServiceOptions] = useState([]);
  const [fieldErrors, setFieldErrors] = useState({});

  const deviceServices = {
    'Phone': [
      'Screen Replacement', 'Battery Replacement', 'Charging Port',
      'Water Damage', 'Software Reinstall', 'Hardware Cleaning', 'Other'
    ],
    'Laptop': [
      'Screen Replacement', 'Battery Replacement', 'Keyboard Replacement',
      'RAM Upgrade', 'Storage Upgrade', 'Software Reinstall', 'Hardware Cleaning', 'Other'
    ],
    'Custom PC Build': [
      'Custom PC Build', 'Hardware Cleaning', 'Diagnostics', 'Other'
    ]
  };

  useEffect(() => {
    if (formData.deviceType) {
      setServiceOptions(deviceServices[formData.deviceType] || []);
      setFormData(prev => ({ ...prev, serviceType: '' }));
    } else {
      setServiceOptions([]);
    }
  }, [formData.deviceType]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const validate = () => {
    const errs = {};
    if (!formData.deviceType) errs.deviceType = 'Device type is required';
    if (!formData.brand.trim()) errs.brand = 'Brand is required';
    if (!formData.serviceType) errs.serviceType = 'Service type is required';
    if (!formData.issueDescription.trim() || formData.issueDescription.length < 20) {
      errs.issueDescription = 'Please provide a detailed description (min 20 chars)';
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
      const response = await serviceRequestService.create(formData);
      setSuccess(`Request submitted successfully! Request ID: ${response.id}`);
      setTimeout(() => navigate('/service-requests'), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to submit request');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container" style={{ padding: '3rem 0' }}>
      <div className="card" style={{ maxWidth: '600px', margin: '0 auto', backgroundColor: 'var(--card-bg)', padding: '2rem', borderRadius: 'var(--radius)', border: '1px solid var(--border)' }}>
        <h1 style={{ marginBottom: '2rem' }}>Request Repair Service</h1>

        {success && <div style={{ backgroundColor: 'rgba(34, 197, 94, 0.1)', color: 'var(--success)', padding: '1rem', borderRadius: '4px', marginBottom: '1.5rem', textAlign: 'center' }}>{success}</div>}
        {error && <div style={{ backgroundColor: 'rgba(239, 68, 68, 0.1)', color: 'var(--danger)', padding: '1rem', borderRadius: '4px', marginBottom: '1.5rem', textAlign: 'center' }}>{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Device Type</label>
            <select name="deviceType" className={`form-control ${fieldErrors.deviceType ? 'error' : ''}`} value={formData.deviceType} onChange={handleChange}>
              <option value="">Select Device Type</option>
              <option value="Phone">Phone</option>
              <option value="Laptop">Laptop</option>
              <option value="Custom PC Build">Custom PC Build</option>
            </select>
            {fieldErrors.deviceType && <p className="error-text">{fieldErrors.deviceType}</p>}
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div className="form-group">
              <label>Brand</label>
              <input name="brand" className={`form-control ${fieldErrors.brand ? 'error' : ''}`} value={formData.brand} onChange={handleChange} placeholder="e.g. Apple, Dell" />
              {fieldErrors.brand && <p className="error-text">{fieldErrors.brand}</p>}
            </div>
            
            <div className="form-group">
              <label>Model</label>
              <input name="model" className="form-control" value={formData.model} onChange={handleChange} placeholder="e.g. iPhone 15, XPS 13" />
            </div>
          </div>

          <div className="form-group">
            <label>Service Required</label>
            <select name="serviceType" className={`form-control ${fieldErrors.serviceType ? 'error' : ''}`} value={formData.serviceType} onChange={handleChange} disabled={!formData.deviceType}>
              <option value="">Select Service Type</option>
              {serviceOptions.map(opt => <option key={opt} value={opt}>{opt}</option>)}
            </select>
            {fieldErrors.serviceType && <p className="error-text">{fieldErrors.serviceType}</p>}
          </div>

          <div className="form-group">
            <label>Issue Description</label>
            <textarea 
              name="issueDescription" 
              className={`form-control ${fieldErrors.issueDescription ? 'error' : ''}`} 
              rows="5" 
              value={formData.issueDescription} 
              onChange={handleChange}
              placeholder="Describe the issue you're facing in detail..."
            ></textarea>
            {fieldErrors.issueDescription && <p className="error-text">{fieldErrors.issueDescription}</p>}
          </div>

          <button type="submit" className="btn btn-primary btn-block btn-lg" disabled={loading}>
            {loading ? 'Submitting...' : 'Submit Request'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default CreateServiceRequestPage;
