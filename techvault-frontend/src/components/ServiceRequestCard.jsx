import React from 'react';
import './ServiceRequestCard.css';

const ServiceRequestCard = ({ request, isAdmin, onStatusChange }) => {
  const { id, deviceType, brand, model, serviceType, status, issueDescription, createdAt } = request;

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-EG');
  };

  const getStatusClass = (status) => {
    const statusMap = {
      'Received': 'badge-received',
      'Diagnosing': 'badge-diagnosing',
      'WaitingParts': 'badge-waitingparts',
      'InProgress': 'badge-inprogress',
      'ReadyForPickup': 'badge-readyforpickup',
      'Completed': 'badge-completed',
      'Cancelled': 'badge-cancelled'
    };
    return statusMap[status] || 'badge-pending';
  };

  return (
    <div className="service-card">
      <div className="service-header">
        <div>
          <span className="service-device">{deviceType}</span>
          <h3 className="service-model">{brand} {model}</h3>
        </div>
        <span className={`badge ${getStatusClass(status)}`}>{status}</span>
      </div>
      
      <div className="service-body">
        <p className="service-type"><strong>Service:</strong> {serviceType}</p>
        <p className="service-desc">{issueDescription}</p>
        <p className="service-date">Submitted on: {formatDate(createdAt)}</p>
      </div>

      {isAdmin && (
        <div className="service-footer">
          <label>Update Status:</label>
          <select 
            value={status} 
            onChange={(e) => onStatusChange(id, e.target.value)}
            className="form-control status-select"
          >
            <option value="Received">Received</option>
            <option value="Diagnosing">Diagnosing</option>
            <option value="WaitingParts">Waiting Parts</option>
            <option value="InProgress">In Progress</option>
            <option value="ReadyForPickup">Ready For Pickup</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      )}
    </div>
  );
};

export default ServiceRequestCard;
