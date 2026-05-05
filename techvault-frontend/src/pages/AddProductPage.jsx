import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import productService from '../services/productService';
import categoryService from '../services/categoryService';

const AddProductPage = () => {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    stockQuantity: '',
    brand: '',
    model: '',
    sku: '',
    imageUrl: '',
    categoryId: '',
    tagIds: []
  });

  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    const fetchCats = async () => {
      try {
        const data = await categoryService.getAll();
        setCategories(data);
      } catch (err) {
        console.error('Failed to load categories');
      }
    };
    fetchCats();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const validate = () => {
    const errs = {};
    if (!formData.name.trim()) errs.name = 'Name is required';
    if (!formData.price || formData.price <= 0) errs.price = 'Price must be greater than 0';
    if (formData.stockQuantity === '' || formData.stockQuantity < 0) errs.stockQuantity = 'Stock must be >= 0';
    if (!formData.brand.trim()) errs.brand = 'Brand is required';
    if (!formData.categoryId) errs.categoryId = 'Category is required';
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
    setSuccess('');

    try {
      await productService.create(formData);
      setSuccess('Product created successfully!');
      setTimeout(() => navigate('/products'), 2000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create product');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container" style={{ padding: '3rem 0' }}>
      <div className="card" style={{ maxWidth: '800px', margin: '0 auto', backgroundColor: 'var(--card-bg)', padding: '2rem', borderRadius: 'var(--radius)', border: '1px solid var(--border)' }}>
        <h1 style={{ marginBottom: '2rem' }}>Add New Product</h1>

        {success && <div style={{ backgroundColor: 'rgba(34, 197, 94, 0.1)', color: 'var(--success)', padding: '1rem', borderRadius: '4px', marginBottom: '1.5rem', textAlign: 'center' }}>{success}</div>}
        {error && <div style={{ backgroundColor: 'rgba(239, 68, 68, 0.1)', color: 'var(--danger)', padding: '1rem', borderRadius: '4px', marginBottom: '1.5rem', textAlign: 'center' }}>{error}</div>}

        <form onSubmit={handleSubmit}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
            <div className="form-group">
              <label>Product Name</label>
              <input name="name" className={`form-control ${fieldErrors.name ? 'error' : ''}`} value={formData.name} onChange={handleChange} />
              {fieldErrors.name && <p className="error-text">{fieldErrors.name}</p>}
            </div>
            
            <div className="form-group">
              <label>Category</label>
              <select name="categoryId" className={`form-control ${fieldErrors.categoryId ? 'error' : ''}`} value={formData.categoryId} onChange={handleChange}>
                <option value="">Select Category</option>
                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
              {fieldErrors.categoryId && <p className="error-text">{fieldErrors.categoryId}</p>}
            </div>

            <div className="form-group">
              <label>Brand</label>
              <input name="brand" className={`form-control ${fieldErrors.brand ? 'error' : ''}`} value={formData.brand} onChange={handleChange} />
              {fieldErrors.brand && <p className="error-text">{fieldErrors.brand}</p>}
            </div>

            <div className="form-group">
              <label>Model</label>
              <input name="model" className="form-control" value={formData.model} onChange={handleChange} />
            </div>

            <div className="form-group">
              <label>Price (EGP)</label>
              <input name="price" type="number" className={`form-control ${fieldErrors.price ? 'error' : ''}`} value={formData.price} onChange={handleChange} />
              {fieldErrors.price && <p className="error-text">{fieldErrors.price}</p>}
            </div>

            <div className="form-group">
              <label>Stock Quantity</label>
              <input name="stockQuantity" type="number" className={`form-control ${fieldErrors.stockQuantity ? 'error' : ''}`} value={formData.stockQuantity} onChange={handleChange} />
              {fieldErrors.stockQuantity && <p className="error-text">{fieldErrors.stockQuantity}</p>}
            </div>
          </div>

          <div className="form-group">
            <label>Image URL</label>
            <input name="imageUrl" className="form-control" value={formData.imageUrl} onChange={handleChange} placeholder="https://example.com/image.jpg" />
          </div>

          <div className="form-group">
            <label>Description</label>
            <textarea name="description" className="form-control" style={{ minHeight: '100px' }} value={formData.description} onChange={handleChange}></textarea>
          </div>



          <div style={{ marginTop: '2rem', display: 'flex', gap: '1rem' }}>
            <button type="submit" className="btn btn-primary flex-grow" disabled={loading}>
              {loading ? 'Creating...' : 'Create Product'}
            </button>
            <button type="button" className="btn btn-outline" onClick={() => navigate('/products')}>
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddProductPage;
