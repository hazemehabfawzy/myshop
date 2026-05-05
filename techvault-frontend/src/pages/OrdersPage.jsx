import React, { useState, useEffect } from 'react';
import orderService from '../services/orderService';
import { useAuth } from '../context/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';
import { formatPrice } from '../utils/format';
import './OrdersPage.css';

const OrdersPage = () => {
  const { isAdmin } = useAuth();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchOrders = async () => {
    setLoading(true);
    try {
      const data = isAdmin 
        ? await orderService.getAll() 
        : await orderService.getMyOrders();
      setOrders(data);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load orders');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, [isAdmin]);

  const handleStatusUpdate = async (id, newStatus) => {
    try {
      await orderService.updateStatus(id, newStatus);
      fetchOrders();
    } catch (err) {
      alert('Failed to update status');
    }
  };

  const handleCancel = async (id) => {
    if (window.confirm('Are you sure you want to cancel this order?')) {
      try {
        await orderService.cancel(id);
        fetchOrders();
      } catch (err) {
        alert('Failed to cancel order');
      }
    }
  };



  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-EG');
  };

  const getStatusBadgeClass = (status) => {
    return `badge badge-${status.toLowerCase()}`;
  };

  if (loading) return <LoadingSpinner message="Loading orders..." />;
  if (error) return <ErrorMessage message={error} onRetry={fetchOrders} />;

  return (
    <div className="container orders-page">
      <h1 className="page-title">{isAdmin ? 'All Customer Orders' : 'My Orders'}</h1>

      {orders.length === 0 ? (
        <div className="empty-state">
          <p>No orders found.</p>
        </div>
      ) : (
        <div className="table-container">
          <table className="orders-table">
            <thead>
              <tr>
                <th>Order ID</th>
                <th>Date</th>
                <th>Status</th>
                <th>Total</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(order => (
                <tr key={order.id}>
                  <td><code className="order-id">#{order.id.substring(0, 8)}</code></td>
                  <td>{formatDate(order.createdAt)}</td>
                  <td>
                    {isAdmin ? (
                      <select 
                        value={order.status} 
                        onChange={(e) => handleStatusUpdate(order.id, e.target.value)}
                        className={`form-control status-select ${getStatusBadgeClass(order.status)}`}
                      >
                        <option value="Pending">Pending</option>
                        <option value="Processing">Processing</option>
                        <option value="Shipped">Shipped</option>
                        <option value="Delivered">Delivered</option>
                        <option value="Cancelled">Cancelled</option>
                      </select>
                    ) : (
                      <span className={`badge ${getStatusBadgeClass(order.status)}`}>
                        {order.status}
                      </span>
                    )}
                  </td>
                  <td className="order-total">{formatPrice(order.totalAmount)}</td>
                  <td>
                    {!isAdmin && (order.status === 'Pending' || order.status === 'Processing') && (
                      <button onClick={() => handleCancel(order.id)} className="btn btn-danger btn-sm">
                        Cancel
                      </button>
                    )}
                    {/* Add View Details functionality if needed */}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default OrdersPage;
