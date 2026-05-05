import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import productService from '../services/productService';
import categoryService from '../services/categoryService';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

const EditProductPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
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
    specificationsJson: '{}',
    tagIds: []
  });

  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [cats, product] = await Promise.all([
          categoryService.getAll(),
          productService.getById(id)
        ]);
        setCategories(cats);
        setFormData({
          name: product.name,
          description: product.description,
          price: product.price,
          stockQuantity: product.stockQuantity,
          brand: product.brand,
          model: product.model,
          sku: product.sku || '',
          imageUrl: product.imageUrl || '',
          categoryId: product.categoryId,
          specificationsJson: JSON.stringify(product.specifications || {}, null, 2),
          tagIds: product.tagIds || []
        });
      } catch (err) {
        setError('Failed to load product data');
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [id]);

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

    setSaving(true);
    setError('');

    try {
      const payload = { ...formData };
      try {
        payload.specifications = JSON.parse(formData.specificationsJson);
      } catch (e) {
        payload.specifications = {};
      }
      delete payload.specificationsJson;

      await productService.update(id, payload);
      setSuccess('Product updated successfully!');
      setTimeout(() => navigate(`/products/${id}`), 1500);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update product');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      try {
        await productService.remove(id);
        navigate('/products');
      } catch (err) {
        setError('Failed to delete product');
      }
    }
  };

  if (loading) return <LoadingSpinner message="Loading product data..." />;

  return (
    <div className="container" style={{ padding: '3rem 0' }}>
      <div className="card" style={{ maxWidth: '800px', margin: '0 auto', backgroundColor: 'var(--card-bg)', padding: '2rem', borderRadius: 'var(--radius)', border: '1px solid var(--border)' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
          <h1>Edit Product</h1>
          <button onClick={handleDelete} className="btn btn-danger">Delete Product</button>
        </div>

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
            <input name="imageUrl" className="form-control" value={formData.imageUrl} onChange={handleChange} />
          </div>

          <div className="form-group">
            <label>Description</label>
            <textarea name="description" className="form-control" style={{ minHeight: '100px' }} value={formData.description} onChange={handleChange}></textarea>
          </div>

          <div className="form-group">
            <label>Specifications (JSON)</label>
            <textarea name="specificationsJson" className="form-control" style={{ fontFamily: 'monospace', minHeight: '150px' }} value={formData.specificationsJson} onChange={handleChange}></textarea>
          </div>

          <div style={{ marginTop: '2rem', display: 'flex', gap: '1rem' }}>
            <button type="submit" className="btn btn-primary flex-grow" disabled={saving}>
              {saving ? 'Saving...' : 'Update Product'}
            </button>
            <button type="button" className="btn btn-outline" onClick={() => navigate(`/products/${id}`)}>
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditProductPage;
