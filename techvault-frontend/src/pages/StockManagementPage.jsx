import React, { useState, useEffect } from 'react';
import productService from '../services/productService';
import categoryService from '../services/categoryService';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';
import { formatPrice } from '../utils/format';

const StockManagementPage = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Search/Filters
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('');

  // Row-level local stock input state
  const [stockInputs, setStockInputs] = useState({});
  const [updatingRows, setUpdatingRows] = useState({});
  const [rowFeedback, setRowFeedback] = useState({});

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [catsData, productsData] = await Promise.all([
        categoryService.getAll(),
        productService.getAll({ pageSize: 100 }) // Load up to 100 items for stock list
      ]);
      setCategories(catsData);
      
      const itemsList = Array.isArray(productsData) ? productsData : productsData.items || [];
      setProducts(itemsList);

      // Initialize row-level stock inputs
      const initialInputs = {};
      itemsList.forEach(p => {
        initialInputs[p.id] = p.stockQuantity;
      });
      setStockInputs(initialInputs);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load stock data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleStockChange = (productId, value) => {
    const qty = parseInt(value);
    if (isNaN(qty) || qty < 0) return;
    setStockInputs(prev => ({ ...prev, [productId]: qty }));
  };

  const handleIncrement = (productId) => {
    const currentVal = stockInputs[productId] !== undefined ? stockInputs[productId] : 0;
    setStockInputs(prev => ({ ...prev, [productId]: currentVal + 1 }));
  };

  const handleDecrement = (productId) => {
    const currentVal = stockInputs[productId] !== undefined ? stockInputs[productId] : 0;
    if (currentVal <= 0) return;
    setStockInputs(prev => ({ ...prev, [productId]: currentVal - 1 }));
  };

  const handleUpdateStock = async (productId) => {
    const newQty = stockInputs[productId];
    if (newQty === undefined || isNaN(newQty) || newQty < 0) {
      setRowFeedback(prev => ({ ...prev, [productId]: { type: 'danger', message: 'Invalid quantity' } }));
      return;
    }

    setUpdatingRows(prev => ({ ...prev, [productId]: true }));
    setRowFeedback(prev => ({ ...prev, [productId]: null }));

    try {
      const response = await productService.update(productId, {
        stockQuantity: newQty
      });

      // Update local product record with returned backend calculation
      setProducts(prev => prev.map(p => p.id === productId ? {
        ...p,
        stockQuantity: response.stockQuantity,
        stockStatus: response.stockStatus
      } : p));

      setRowFeedback(prev => ({ ...prev, [productId]: { type: 'success', message: 'Updated!' } }));
      setTimeout(() => {
        setRowFeedback(prev => ({ ...prev, [productId]: null }));
      }, 3000);
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.response?.data?.innerMessage || err.message || 'Failed to update';
      console.error('[StockManagementPage ERROR]', errorMsg, err);
      setRowFeedback(prev => ({ ...prev, [productId]: { type: 'danger', message: errorMsg } }));
    } finally {
      setUpdatingRows(prev => ({ ...prev, [productId]: false }));
    }
  };

  // Filter in-memory for lightning fast search and filter response
  const filteredProducts = products.filter(p => {
    const matchesSearch = p.name.toLowerCase().includes(searchTerm.toLowerCase()) || 
                          p.brand.toLowerCase().includes(searchTerm.toLowerCase()) ||
                          (p.categoryName && p.categoryName.toLowerCase().includes(searchTerm.toLowerCase()));
    
    const matchesCategory = selectedCategory === '' || p.categoryId === selectedCategory;
    
    return matchesSearch && matchesCategory;
  });

  const getStatusBadge = (status, quantity) => {
    if (quantity === 0 || status === 'OutOfStock') {
      return (
        <span style={{
          backgroundColor: 'rgba(239, 68, 68, 0.1)',
          color: 'var(--danger)',
          padding: '0.4rem 0.8rem',
          borderRadius: '50px',
          fontSize: '0.8rem',
          fontWeight: '700',
          border: '1px solid rgba(239, 68, 68, 0.2)'
        }}>Out of Stock</span>
      );
    }
    if (quantity <= 5 || status === 'LowStock') {
      return (
        <span style={{
          backgroundColor: 'rgba(245, 158, 11, 0.1)',
          color: '#f59e0b',
          padding: '0.4rem 0.8rem',
          borderRadius: '50px',
          fontSize: '0.8rem',
          fontWeight: '700',
          border: '1px solid rgba(245, 158, 11, 0.2)'
        }}>Low Stock</span>
      );
    }
    return (
      <span style={{
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        color: 'var(--success)',
        padding: '0.4rem 0.8rem',
        borderRadius: '50px',
        fontSize: '0.8rem',
        fontWeight: '700',
        border: '1px solid rgba(34, 197, 94, 0.2)'
      }}>In Stock</span>
    );
  };

  if (loading) return <LoadingSpinner message="Loading stock inventory details..." />;
  if (error) return <ErrorMessage message={error} onRetry={fetchData} />;

  return (
    <div className="container" style={{ padding: '3rem 0' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2.5rem' }}>
        <div>
          <h1 style={{ margin: 0, fontSize: '2.25rem', fontWeight: '800' }}>Stock Management</h1>
          <p style={{ color: 'var(--text-muted)', marginTop: '0.5rem' }}>Monitor and update physical product quantities in real-time</p>
        </div>
        <button onClick={fetchData} className="btn btn-outline" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          🔄 Sync Stock
        </button>
      </div>

      {/* Filter and Search Bar */}
      <div className="card" style={{
        backgroundColor: 'var(--card-bg)',
        border: '1px solid var(--border)',
        borderRadius: 'var(--radius)',
        padding: '1.5rem',
        marginBottom: '2rem',
        display: 'flex',
        gap: '1.5rem',
        alignItems: 'center',
        flexWrap: 'wrap'
      }}>
        <div style={{ flex: 1, minWidth: '250px' }}>
          <input
            type="text"
            className="form-control"
            placeholder="Search by product name, brand, or category..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            style={{ width: '100%' }}
          />
        </div>
        <div style={{ width: '200px' }}>
          <select
            className="form-control"
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            style={{ width: '100%' }}
          >
            <option value="">All Categories</option>
            {categories.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>
        <div style={{ color: 'var(--text-muted)', fontSize: '0.9rem', fontWeight: '500' }}>
          Showing {filteredProducts.length} of {products.length} products
        </div>
      </div>

      {/* Stock Inventory Table */}
      <div className="card" style={{
        backgroundColor: 'var(--card-bg)',
        border: '1px solid var(--border)',
        borderRadius: 'var(--radius)',
        overflow: 'hidden',
        padding: 0
      }}>
        {filteredProducts.length === 0 ? (
          <div style={{ padding: '4rem 2rem', textAlign: 'center', color: 'var(--text-muted)' }}>
            <h3>No products found</h3>
            <p style={{ marginTop: '0.5rem' }}>Try adjusting your search criteria or category filter.</p>
          </div>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid var(--border)', backgroundColor: 'var(--primary-light)' }}>
                <th style={{ padding: '1.25rem 1.5rem', fontWeight: '700', fontSize: '0.9rem', color: 'var(--text-muted)', textTransform: 'uppercase' }}>Product Name</th>
                <th style={{ padding: '1.25rem 1.5rem', fontWeight: '700', fontSize: '0.9rem', color: 'var(--text-muted)', textTransform: 'uppercase' }}>Brand</th>
                <th style={{ padding: '1.25rem 1.5rem', fontWeight: '700', fontSize: '0.9rem', color: 'var(--text-muted)', textTransform: 'uppercase' }}>Category</th>
                <th style={{ padding: '1.25rem 1.5rem', fontWeight: '700', fontSize: '0.9rem', color: 'var(--text-muted)', textTransform: 'uppercase' }}>Current Stock</th>
                <th style={{ padding: '1.25rem 1.5rem', fontWeight: '700', fontSize: '0.9rem', color: 'var(--text-muted)', textTransform: 'uppercase' }}>Stock Status</th>
                <th style={{ padding: '1.25rem 1.5rem', fontWeight: '700', fontSize: '0.9rem', color: 'var(--text-muted)', textTransform: 'uppercase', textAlign: 'right' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredProducts.map(p => {
                const isUpdating = updatingRows[p.id];
                const feedback = rowFeedback[p.id];
                const currentInputValue = stockInputs[p.id] !== undefined ? stockInputs[p.id] : p.stockQuantity;

                return (
                  <tr key={p.id} style={{ borderBottom: '1px solid var(--border)', transition: 'background-color 0.25s ease' }} className="hover-row">
                    {/* Name */}
                    <td style={{ padding: '1.25rem 1.5rem' }}>
                      <div style={{ fontWeight: '700', fontSize: '1rem', color: 'var(--text)' }}>{p.name}</div>
                      <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>EGP {formatPrice(p.price)}</div>
                    </td>

                    {/* Brand */}
                    <td style={{ padding: '1.25rem 1.5rem' }}>
                      <span style={{
                        fontSize: '0.8rem',
                        fontWeight: '700',
                        color: 'var(--accent)',
                        backgroundColor: 'rgba(96, 165, 250, 0.05)',
                        padding: '0.25rem 0.6rem',
                        borderRadius: '4px',
                        border: '1px solid rgba(96, 165, 250, 0.1)',
                        textTransform: 'uppercase',
                        letterSpacing: '0.05em'
                      }}>{p.brand}</span>
                    </td>

                    {/* Category */}
                    <td style={{ padding: '1.25rem 1.5rem', color: 'var(--text)', fontWeight: '500' }}>
                      {p.categoryName}
                    </td>

                    {/* Stock */}
                    <td style={{ padding: '1.25rem 1.5rem', fontSize: '1.1rem', fontWeight: '800' }}>
                      {p.stockQuantity}
                    </td>

                    {/* Status */}
                    <td style={{ padding: '1.25rem 1.5rem' }}>
                      {getStatusBadge(p.stockStatus, p.stockQuantity)}
                    </td>

                    {/* Actions */}
                    <td style={{ padding: '1.25rem 1.5rem', textAlign: 'right' }}>
                      <div style={{ display: 'inline-flex', alignItems: 'center', gap: '0.75rem', justifyContent: 'flex-end' }}>
                        {/* Feedback Pop */}
                        {feedback && (
                          <span style={{
                            fontSize: '0.8rem',
                            fontWeight: '700',
                            color: feedback.type === 'success' ? 'var(--success)' : 'var(--danger)',
                            marginRight: '0.25rem'
                          }}>{feedback.message}</span>
                        )}

                        {/* Spinner Widget */}
                        <div style={{
                          display: 'flex',
                          alignItems: 'center',
                          border: '1px solid var(--border)',
                          borderRadius: 'var(--radius)',
                          overflow: 'hidden',
                          backgroundColor: 'var(--primary-dark)',
                          height: '38px'
                        }}>
                          <button
                            type="button"
                            onClick={() => handleDecrement(p.id)}
                            disabled={isUpdating || currentInputValue <= 0}
                            style={{
                              width: '32px',
                              height: '100%',
                              backgroundColor: 'transparent',
                              border: 'none',
                              color: 'var(--text)',
                              fontWeight: 'bold',
                              fontSize: '1rem',
                              cursor: 'pointer',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              userSelect: 'none',
                              transition: 'background-color 0.15s'
                            }}
                            onMouseOver={(e) => e.currentTarget.style.backgroundColor = 'rgba(255,255,255,0.05)'}
                            onMouseOut={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
                          >
                            −
                          </button>
                          <input
                            type="number"
                            value={currentInputValue}
                            onChange={(e) => handleStockChange(p.id, e.target.value)}
                            disabled={isUpdating}
                            style={{
                              width: '48px',
                              height: '100%',
                              textAlign: 'center',
                              backgroundColor: 'transparent',
                              border: 'none',
                              color: 'var(--text)',
                              fontWeight: '700',
                              fontSize: '0.95rem',
                              appearance: 'none',
                              outline: 'none',
                              borderLeft: '1px solid var(--border)',
                              borderRight: '1px solid var(--border)'
                            }}
                          />
                          <button
                            type="button"
                            onClick={() => handleIncrement(p.id)}
                            disabled={isUpdating}
                            style={{
                              width: '32px',
                              height: '100%',
                              backgroundColor: 'transparent',
                              border: 'none',
                              color: 'var(--text)',
                              fontWeight: 'bold',
                              fontSize: '1rem',
                              cursor: 'pointer',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              userSelect: 'none',
                              transition: 'background-color 0.15s'
                            }}
                            onMouseOver={(e) => e.currentTarget.style.backgroundColor = 'rgba(255,255,255,0.05)'}
                            onMouseOut={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
                          >
                            +
                          </button>
                        </div>

                        {/* Submit Button */}
                        <button
                          type="button"
                          className="btn btn-primary"
                          onClick={() => handleUpdateStock(p.id)}
                          disabled={isUpdating || currentInputValue === p.stockQuantity}
                          style={{
                            height: '38px',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            whiteSpace: 'nowrap',
                            fontSize: '0.85rem',
                            fontWeight: '700',
                            padding: '0 1rem'
                          }}
                        >
                          {isUpdating ? 'Updating...' : 'Update Stock'}
                        </button>
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>
      
      {/* Custom Styles Injection */}
      <style>{`
        .hover-row:hover {
          background-color: rgba(255, 255, 255, 0.02) !important;
        }
        input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
          -webkit-appearance: none; 
          margin: 0; 
        }
        input[type=number] {
          -moz-appearance: textfield;
        }
      `}</style>
    </div>
  );
};

export default StockManagementPage;
