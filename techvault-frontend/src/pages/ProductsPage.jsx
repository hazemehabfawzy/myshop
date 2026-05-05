import React, { useState, useEffect } from 'react';
import productService from '../services/productService';
import categoryService from '../services/categoryService';
import ProductCard from '../components/ProductCard';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';
import './ProductsPage.css';

const ProductsPage = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [filters, setFilters] = useState({
    categoryId: '',
    minPrice: '',
    maxPrice: '',
    search: '',
    pageNumber: 1,
    pageSize: 8
  });

  const fetchCategories = async () => {
    try {
      const data = await categoryService.getAll();
      setCategories(data);
    } catch (err) {
      console.error('Failed to fetch categories', err);
    }
  };

  const fetchProducts = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await productService.getAll(filters);
      // Backend returns a paginated result, usually { items: [], totalCount: X, ... }
      // Or just an array if not paginated. I'll assume an array or check structure.
      setProducts(Array.isArray(data) ? data : data.items || []);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load products');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  useEffect(() => {
    fetchProducts();
  }, [filters]);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters(prev => ({ ...prev, [name]: value, pageNumber: 1 }));
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchProducts();
  };

  return (
    <div className="container products-page">
      <h1 className="page-title">Browse Our Collection</h1>

      <div className="filter-bar">
        <form className="search-form" onSubmit={handleSearch}>
          <input 
            type="text" 
            name="search"
            placeholder="Search products..." 
            className="form-control"
            value={filters.search}
            onChange={handleFilterChange}
          />
        </form>

        <div className="filters-group">
          <select 
            name="categoryId" 
            className="form-control" 
            value={filters.categoryId}
            onChange={handleFilterChange}
          >
            <option value="">All Categories</option>
            {categories.map(cat => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </select>

          <input 
            type="number" 
            name="minPrice"
            placeholder="Min Price" 
            className="form-control price-input"
            value={filters.minPrice}
            onChange={handleFilterChange}
          />

          <input 
            type="number" 
            name="maxPrice"
            placeholder="Max Price" 
            className="form-control price-input"
            value={filters.maxPrice}
            onChange={handleFilterChange}
          />
        </div>
      </div>

      {loading ? (
        <LoadingSpinner message="Fetching products..." />
      ) : error ? (
        <ErrorMessage message={error} onRetry={fetchProducts} />
      ) : products.length === 0 ? (
        <div className="no-results">
          <p>No products found matching your criteria.</p>
          <button className="btn btn-primary" onClick={() => setFilters({
            categoryId: '', minPrice: '', maxPrice: '', search: '', pageNumber: 1, pageSize: 8
          })}>
            Clear Filters
          </button>
        </div>
      ) : (
        <>
          <div className="products-grid">
            {products.map(product => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          <div className="pagination">
            <button 
              className="btn btn-outline" 
              disabled={filters.pageNumber === 1}
              onClick={() => setFilters(prev => ({ ...prev, pageNumber: prev.pageNumber - 1 }))}
            >
              Previous
            </button>
            <span className="page-info">Page {filters.pageNumber}</span>
            <button 
              className="btn btn-outline" 
              disabled={products.length < filters.pageSize}
              onClick={() => setFilters(prev => ({ ...prev, pageNumber: prev.pageNumber + 1 }))}
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default ProductsPage;
