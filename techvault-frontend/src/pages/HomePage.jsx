import React from 'react';
import { Link } from 'react-router-dom';
import './HomePage.css';

const HomePage = () => {
  return (
    <div className="homepage">
      <section className="hero">
        <div className="container hero-content">
          <h1 className="hero-title">TechVault — Your electronics & repair hub</h1>
          <p className="hero-subtitle">
            Find the latest tech gadgets and professional repair services all in one place.
          </p>
          <div className="hero-actions">
            <Link to="/products" className="btn btn-primary btn-lg">Browse Products</Link>
            <Link to="/service-requests/new" className="btn btn-outline btn-lg">Request Repair</Link>
          </div>
        </div>
      </section>

      <section className="features container">
        <div className="feature-card">
          <div className="feature-icon">💻</div>
          <h3>Premium Hardware</h3>
          <p>Laptops, PC components, and the latest accessories.</p>
        </div>
        <div className="feature-card">
          <div className="feature-icon">🛠️</div>
          <h3>Expert Repairs</h3>
          <p>Professional service for phones, laptops, and custom PC builds.</p>
        </div>
        <div className="feature-card">
          <div className="feature-icon">🚚</div>
          <h3>Fast Delivery</h3>
          <p>Quick shipping across Egypt with secure payment options.</p>
        </div>
      </section>
    </div>
  );
};

export default HomePage;
