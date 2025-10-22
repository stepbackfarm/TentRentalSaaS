import React from 'react';
import { Link } from 'react-router-dom';
import { useTheme } from '../context/ThemeContext';

function Navbar() {
    const { theme, toggleTheme } = useTheme();

    return (
        <nav className="bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-gray-100 p-4 shadow-lg">
            <div className="w-full max-w-6xl mx-auto flex justify-between items-center">
                <Link to="/" className="text-2xl font-bold text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300">TentRental</Link>
                <div className="flex gap-4 items-center">
                    <Link to="/" className="hover:text-blue-700 dark:hover:text-blue-300">Home</Link>
                    <Link to="/gallery" className="hover:text-blue-700 dark:hover:text-blue-300">Gallery</Link>
                    <Link to="/faq" className="hover:text-blue-700 dark:hover:text-blue-300">FAQ</Link>
                    <Link to="/portal/login-request" className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                        Customer Portal
                    </Link>
                    <button
                        type="button"
                        onClick={toggleTheme}
                        className="p-2 rounded-full bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 hover:bg-gray-300 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-300 dark:focus:ring-gray-600"
                        aria-label="Toggle theme"
                        data-testid="theme-toggle"
                    >
                        {theme === 'dark' ? '☀️' : '🌙'}
                    </button>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
