import React from 'react';
import { Link } from 'react-router-dom';
import { useTheme } from '../context/ThemeContext';

function Navbar() {
    const { theme, toggleTheme } = useTheme();

    return (
        <nav className="bg-bg-muted text-text-base p-4 shadow-lg">
            <div className="w-full max-w-6xl mx-auto flex justify-between items-center">
                <Link to="/" className="text-2xl font-bold text-primary hover:text-primary-dark">TentRental</Link>
                <div className="flex gap-4 items-center">
                    <Link to="/" className="hover:text-primary-dark">Home</Link>
                    <Link to="/gallery" className="hover:text-primary-dark">Gallery</Link>
                    <Link to="/faq" className="hover:text-primary-dark">FAQ</Link>
                    <Link to="/portal/login-request" className="bg-primary hover:bg-primary-dark text-white font-bold py-2 px-4 rounded">
                        Customer Portal
                    </Link>
                    <button
                        onClick={toggleTheme}
                        className="p-2 rounded-full bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 focus:outline-none focus:ring-2 focus:ring-gray-300 dark:focus:ring-gray-600"
                        aria-label="Toggle theme"
                    >
                        {theme === 'dark' ? '☀️' : '🌙'}
                    </button>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
