import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import productService from '../services/productService';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';
import { useAuth } from '../context/AuthContext';
import { formatPrice } from '../utils/format';
import './ProductDetailPage.css';

const ProductDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [quantity, setQuantity] = useState(1);
  const [imgSrc, setImgSrc] = useState('');

  const fetchProduct = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await productService.getById(id);
      setProduct(data);
      setImgSrc(data.imageUrl || `https://placehold.co/600x400/1e293b/60a5fa?text=${encodeURIComponent(data.name)}`);
    } catch (err) {
      setError(err.response?.data?.message || 'Product not found');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProduct();
  }, [id]);

  const handleAddToOrder = () => {
    navigate('/orders/new', { 
      state: { 
        productId: product.id, 
        productName: product.name, 
        price: product.price, 
        quantity 
      } 
    });
  };

  if (loading) return <LoadingSpinner message="Loading product details..." />;
  if (error) return <ErrorMessage message={error} onRetry={fetchProduct} />;
  if (!product) return <div>Product not found</div>;

  return (
    <div className="container product-detail-page" style={{ maxWidth: '800px' }}>
      <div className="product-layout-single">
        <div className="product-details">
          <div className="detail-header">
            <span className="detail-brand">{product.brand}</span>
            <h1 className="detail-name">{product.name}</h1>
            <span className="badge badge-primary">{product.categoryName}</span>
          </div>

          <p className="detail-description">{product.description}</p>

          <div className="detail-price-section">
            <span className="detail-price">{formatPrice(product.price)}</span>
            {product.stockQuantity > 0 ? (
              <span className="badge badge-delivered">In Stock ({product.stockQuantity} available)</span>
            ) : (
              <span className="badge badge-cancelled">Out of Stock</span>
            )}
          </div>

          <div className="detail-actions">
            <div className="quantity-selector">
              <button 
                onClick={() => setQuantity(Math.max(1, quantity - 1))}
                disabled={product.stockQuantity === 0}
              >-</button>
              <span>{quantity}</span>
              <button 
                onClick={() => setQuantity(Math.min(product.stockQuantity, quantity + 1))}
                disabled={product.stockQuantity === 0}
              >+</button>
            </div>
            
            <button 
              className="btn btn-primary btn-lg flex-grow"
              onClick={handleAddToOrder}
              disabled={product.stockQuantity === 0}
            >
              Add to Order
            </button>
          </div>

          {isAdmin && (
            <Link to={`/admin/products/${product.id}/edit`} className="btn btn-outline btn-block" style={{ marginTop: '1rem' }}>
              Edit Product
            </Link>
          )}



          <div className="detail-tags">
            {product.tags && product.tags.map(tag => (
              <span key={tag.id} className="tag-pill">#{tag.name}</span>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailPage;
