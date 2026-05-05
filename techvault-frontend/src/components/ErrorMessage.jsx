import React from 'react';

const ErrorMessage = ({ message, onRetry }) => {
  return (
    <div className="container" style={{ padding: '2rem 0' }}>
      <div style={{
        backgroundColor: 'rgba(239, 68, 68, 0.1)',
        border: '1px solid var(--danger)',
        padding: '1.5rem',
        borderRadius: 'var(--radius)',
        textAlign: 'center'
      }}>
        <h3 style={{ color: 'var(--danger)', marginBottom: '0.5rem' }}>Error</h3>
        <p style={{ color: 'var(--text)', marginBottom: '1rem' }}>{message}</p>
        {onRetry && (
          <button className="btn btn-primary" onClick={onRetry}>
            Try Again
          </button>
        )}
      </div>
    </div>
  );
};

export default ErrorMessage;
