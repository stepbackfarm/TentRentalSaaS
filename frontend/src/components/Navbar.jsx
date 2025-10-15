import React from 'react';
import { Link } from 'react-router-dom';

function Navbar() {
    return (
        <nav className="bg-gray-800 text-white p-4 shadow-lg">
            <div className="w-full max-w-6xl mx-auto flex justify-between items-center">
                <Link to="/" className="text-2xl font-bold text-blue-400 hover:text-blue-300">TentRental</Link>
                <div className="flex gap-4">
                    <Link to="/" className="hover:text-blue-300">Home</Link>
                    <Link to="/gallery" className="hover:text-blue-300">Gallery</Link>
                    <Link to="/faq" className="hover:text-blue-300">FAQ</Link>
                    <Link to="/portal/login-request" className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                        Customer Portal
                    </Link>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
