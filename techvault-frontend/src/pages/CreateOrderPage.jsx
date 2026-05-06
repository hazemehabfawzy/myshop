import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import productService from '../services/productService';
import orderService from '../services/orderService';
import LoadingSpinner from '../components/LoadingSpinner';
import { formatPrice } from '../utils/format';

const CreateOrderPage = () => {
  const location = useLocation();
  const navigate = useNavigate();
  
  const [selectedItems, setSelectedItems] = useState([]);
  const [shippingAddress, setShippingAddress] = useState('');
  const [paymentMethod, setPaymentMethod] = useState('CashOnDelivery');
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);

  useEffect(() => {
    if (location.state) {
      const { productId, productName, price, quantity } = location.state;
      setSelectedItems([{ productId, productName, price, quantity }]);
    }
  }, [location.state]);

  const handleSearch = async (e) => {
    const query = e.target.value;
    setSearchQuery(query);
    if (query.length > 2) {
      try {
        const results = await productService.search(query);
        setSearchResults(results.items || results);
      } catch (err) {
        console.error('Search failed');
      }
    } else {
      setSearchResults([]);
    }
  };

  const addItem = (product) => {
    if (product.stockQuantity <= 0 || product.stockStatus === 'OutOfStock') {
      setError("This product is currently out of stock.");
      setTimeout(() => setError(''), 5000);
      setSearchQuery('');
      setSearchResults([]);
      return;
    }
    const exists = selectedItems.find(i => i.productId === product.id);
    if (exists) {
      updateQuantity(product.id, exists.quantity + 1);
    } else {
      setSelectedItems([...selectedItems, { 
        productId: product.id, 
        productName: product.name, 
        price: product.price, 
        quantity: 1 
      }]);
    }
    setSearchQuery('');
    setSearchResults([]);
  };

  const updateQuantity = (id, q) => {
    if (q < 1) return;
    setSelectedItems(prev => prev.map(item => 
      item.productId === id ? { ...item, quantity: q } : item
    ));
  };

  const removeItem = (id) => {
    setSelectedItems(prev => prev.filter(i => i.productId !== id));
  };

  const calculateTotal = () => {
    return selectedItems.reduce((acc, item) => acc + (item.price * item.quantity), 0);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (selectedItems.length === 0) {
      setError('Please add at least one item to your order.');
      return;
    }
    if (!shippingAddress.trim()) {
      setError('Shipping address is required.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const orderData = {
        items: selectedItems.map(i => ({ productId: i.productId, quantity: i.quantity })),
        shippingAddress,
        paymentMethod
      };
      const response = await orderService.create(orderData);
      setSuccess(`Order placed successfully! Order ID: ${response.id}`);
      setTimeout(() => navigate('/orders'), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to place order');
    } finally {
      setLoading(false);
    }
  };



  return (
    <div className="container" style={{ padding: '3rem 0' }}>
      <h1 style={{ marginBottom: '2.5rem' }}>Create New Order</h1>

      <div style={{ display: 'grid', gridTemplateColumns: '1.5fr 1fr', gap: '3rem' }}>
        <div>
          <div className="card" style={{ backgroundColor: 'var(--card-bg)', padding: '2rem', borderRadius: 'var(--radius)', border: '1px solid var(--border)', marginBottom: '2rem' }}>
            <h3 style={{ marginBottom: '1.5rem' }}>Add Products</h3>
            {/* Search Input and dropdown removed as requested */}

            <div style={{ marginTop: '2rem' }}>
              {selectedItems.length === 0 ? (
                <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>No items added yet.</p>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                  {selectedItems.map(item => (
                    <div key={item.productId} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '1rem', border: '1px solid var(--border)', borderRadius: 'var(--radius)' }}>
                      <div style={{ flex: 1 }}>
                        <h4 style={{ fontSize: '1rem' }}>{item.productName}</h4>
                        <span style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>{formatPrice(item.price)} each</span>
                      </div>
                      
                      <div style={{ display: 'flex', alignItems: 'center', gap: '1.5rem' }}>
                        <div className="quantity-selector" style={{ height: '35px' }}>
                          <button onClick={() => updateQuantity(item.productId, item.quantity - 1)}>-</button>
                          <span>{item.quantity}</span>
                          <button onClick={() => updateQuantity(item.productId, item.quantity + 1)}>+</button>
                        </div>
                        <span style={{ fontWeight: '700', width: '100px', textAlign: 'right' }}>{formatPrice(item.price * item.quantity)}</span>
                        <button onClick={() => removeItem(item.productId)} style={{ color: 'var(--danger)', background: 'none' }}>Remove</button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        <div>
          <div className="card" style={{ backgroundColor: 'var(--card-bg)', padding: '2rem', borderRadius: 'var(--radius)', border: '1px solid var(--border)', position: 'sticky', top: '100px' }}>
            <h3 style={{ marginBottom: '1.5rem' }}>Order Summary</h3>
            
            <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem', marginBottom: '2rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <span color="var(--text-muted)">Subtotal</span>
                <span>{formatPrice(calculateTotal())}</span>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <span color="var(--text-muted)">Shipping</span>
                <span style={{ color: 'var(--success)' }}>Free</span>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between', borderTop: '1px solid var(--border)', paddingTop: '1rem', fontSize: '1.25rem', fontWeight: '800' }}>
                <span>Total</span>
                <span style={{ color: 'var(--accent)' }}>{formatPrice(calculateTotal())}</span>
              </div>
            </div>

            {error && <div className="error-text" style={{ marginBottom: '1rem', textAlign: 'center' }}>{error}</div>}
            {success && <div style={{ marginBottom: '1rem', textAlign: 'center', color: 'var(--success)' }}>{success}</div>}

            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Shipping Address</label>
                <textarea 
                  className="form-control" 
                  rows="3" 
                  value={shippingAddress} 
                  onChange={(e) => setShippingAddress(e.target.value)}
                  placeholder="Enter full address for delivery"
                ></textarea>
              </div>

              <div className="form-group">
                <label>Payment Method</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value="Cash on Delivery" 
                  readOnly 
                  style={{ backgroundColor: 'var(--primary)', cursor: 'not-allowed', fontWeight: '600', color: 'var(--text)' }}
                />
              </div>

              <button type="submit" className="btn btn-primary btn-block btn-lg" disabled={loading || selectedItems.length === 0}>
                {loading ? 'Processing...' : 'Place Order'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateOrderPage;
