import React, { useState } from 'react';
import { login } from '../services/api';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleLoginRequest = async (e) => {
        e.preventDefault();
        setMessage('');
        setError('');

        try {
            const response = await login(email);
            setMessage(response.message);
        } catch (err) {
            setError('An error occurred. Please try again.');
        }
    };

    return (
        <div className="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-white flex flex-col items-center justify-center p-4">
            <div className="w-full max-w-md bg-gray-100 dark:bg-gray-800 p-8 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700">
                <h1 className="text-3xl font-bold text-center text-blue-600 dark:text-blue-400 mb-6">Customer Portal</h1>
                <p className="text-center text-gray-600 dark:text-gray-400 mb-6">Enter your email address to receive a secure login link. The link will be valid for 15 minutes.</p>
                <form onSubmit={handleLoginRequest}>
                    <div className="mb-4">
                        <label htmlFor="email" className="block text-sm font-bold mb-2 text-gray-900 dark:text-white">Email Address</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full p-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white rounded border border-gray-300 dark:border-gray-600 focus:outline-none focus:border-blue-500 dark:focus:border-blue-400"
                            required
                        />
                    </div>
                    <button type="submit" className="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                        Send Login Link
                    </button>
                </form>
                {message && <p className="mt-4 text-green-600 dark:text-green-400 text-center">{message}</p>}
                {error && <p className="mt-4 text-red-600 dark:text-red-400 text-center">{error}</p>}
            </div>
        </div>
    );
}

export default LoginPage;
