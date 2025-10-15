import React, { useState } from 'react';
import axios from 'axios';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleLoginRequest = async (e) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            const response = await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/auth/login`, { email });
            setMessage(response.data.message);
        } catch (err) {
            setError('An error occurred. Please try again.');
        }
    };

    return (
        <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-4">
            <div className="w-full max-w-md bg-gray-800 p-8 rounded-lg shadow-lg">
                <h1 className="text-3xl font-bold text-center text-blue-400 mb-6">Customer Portal</h1>
                <p className="text-center text-gray-400 mb-6">Enter your email address to receive a secure login link. The link will be valid for 15 minutes.</p>
                <form onSubmit={handleLoginRequest}>
                    <div className="mb-4">
                        <label htmlFor="email" className="block text-sm font-bold mb-2">Email Address</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full p-2 bg-gray-700 rounded border border-gray-600 focus:outline-none focus:border-blue-500"
                            required
                        />
                    </div>
                    <button type="submit" className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                        Send Login Link
                    </button>
                </form>
                {message && <p className="mt-4 text-green-400 text-center">{message}</p>}
                {error && <p className="mt-4 text-red-400 text-center">{error}</p>}
            </div>
        </div>
    );
}

export default LoginPage;
