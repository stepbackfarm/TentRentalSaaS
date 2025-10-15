import React, { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import axios from 'axios';

function PortalLoginPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [error, setError] = useState(null);

    useEffect(() => {
        const token = searchParams.get('token');

        if (!token) {
            setError('No login token provided. Please request a new login link.');
            return;
        }

        const verifyToken = async () => {
            try {
                const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/auth/verify?token=${token}`);
                // On success, redirect to the portal page with the fetched data
                navigate('/portal', { state: { portalData: response.data }, replace: true });
            } catch (err) {
                setError('The login link is invalid or has expired. Please request a new one.');
            }
        };

        verifyToken();
    }, [searchParams, navigate]);

    return (
        <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-4">
            <div className="w-full max-w-md bg-gray-800 p-8 rounded-lg shadow-lg text-center">
                {error ? (
                    <>
                        <h1 className="text-3xl font-bold text-red-500 mb-4">Login Failed</h1>
                        <p className="text-gray-300">{error}</p>
                    </>
                ) : (
                    <>
                        <h1 className="text-3xl font-bold text-blue-400 mb-4">Verifying...</h1>
                        <p className="text-gray-300">Please wait while we securely log you in.</p>
                    </>
                )}
            </div>
        </div>
    );
}

export default PortalLoginPage;
