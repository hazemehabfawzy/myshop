import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { formatPrice } from '../utils/format';
import './ProductCard.css';

const ProductCard = ({ product }) => {
  const { id, name, brand, price, imageUrl, categoryName, stockQuantity } = product;

  const [imgSrc, setImgSrc] = useState(
    imageUrl || `https://placehold.co/400x300/1e293b/60a5fa?text=${encodeURIComponent(name)}`
  );

  const getStockStatus = () => {
    if (stockQuantity <= 0) return { label: 'Out of Stock', class: 'badge-cancelled' };
    if (stockQuantity < 10) return { label: 'Low Stock', class: 'badge-shipped' };
    return { label: 'In Stock', class: 'badge-delivered' };
  };

  const stockStatus = getStockStatus();

  return (
    <div className="product-card">
      <div className="product-info">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
          <span className="product-brand">{brand}</span>
          <span className="badge" style={{ backgroundColor: 'var(--primary-light)', color: 'var(--accent)', border: '1px solid var(--border)', fontSize: '0.75rem', padding: '0.15rem 0.5rem' }}>{categoryName}</span>
        </div>
        <h3 className="product-name">{name}</h3>
        <div className="product-meta">
          <span className={`badge ${stockStatus.class}`}>{stockStatus.label}</span>
          <span className="product-price">{formatPrice(price)}</span>
        </div>
        <Link to={`/products/${id}`} className="btn btn-primary btn-block">
          View Details
        </Link>
      </div>
    </div>
  );
};

export default ProductCard;
