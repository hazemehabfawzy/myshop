import React, { useState, useEffect } from 'react';
import serviceRequestService from '../services/serviceRequestService';
import ServiceRequestCard from '../components/ServiceRequestCard';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';
import { useAuth } from '../context/AuthContext';
import { Link } from 'react-router-dom';

const ServiceRequestsPage = () => {
  const { isAdmin, user } = useAuth();
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const isTechnician = user?.role === 'Technician' || isAdmin;

  const fetchRequests = async () => {
    setLoading(true);
    try {
      const data = isTechnician 
        ? await serviceRequestService.getAll() 
        : await serviceRequestService.getMy();
      setRequests(data);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load service requests');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();
  }, [isAdmin, user?.role]);

  const handleStatusUpdate = async (id, newStatus) => {
    try {
      await serviceRequestService.updateStatus(id, newStatus);
      fetchRequests();
    } catch (err) {
      alert('Failed to update status');
    }
  };

  if (loading) return <LoadingSpinner message="Loading service requests..." />;
  if (error) return <ErrorMessage message={error} onRetry={fetchRequests} />;

  return (
    <div className="container" style={{ padding: '3rem 0' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2.5rem' }}>
        <h1>{isTechnician ? 'All Service Requests' : 'My Service Requests'}</h1>
        {!isTechnician && (
          <Link to="/service-requests/new" className="btn btn-primary">
            Submit New Request
          </Link>
        )}
      </div>

      {requests.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '5rem 0', color: 'var(--text-muted)' }}>
          <p>No service requests found.</p>
        </div>
      ) : (
        <div className="requests-list">
          {requests.map(request => (
            <ServiceRequestCard 
              key={request.id} 
              request={request} 
              isAdmin={isTechnician} 
              onStatusChange={handleStatusUpdate}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default ServiceRequestsPage;
