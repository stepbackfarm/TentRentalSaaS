import React from 'react';
import { useLocation, Navigate } from 'react-router-dom';

function PortalPage() {
    const location = useLocation();
    const portalData = location.state?.portalData;

    // If a user navigates here directly without valid login data, redirect them.
    if (!portalData) {
        return <Navigate to="/portal/login" replace />;
    }

    const { customerName, bookings } = portalData;

    return (
        <div className="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-white p-4 sm:p-8">
            <div className="w-full max-w-4xl mx-auto bg-gray-100 dark:bg-gray-800 p-6 sm:p-8 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700">
                <h1 className="text-3xl sm:text-4xl font-bold text-center text-blue-600 dark:text-blue-400 mb-6">Customer Portal</h1>
                <h2 className="text-2xl text-center text-gray-700 dark:text-gray-300 mb-8">Welcome, {customerName}!</h2>

                <h3 className="text-xl font-semibold text-blue-600 dark:text-blue-300 mb-4">Your Bookings</h3>
                {bookings && bookings.length > 0 ? (
                    <div className="space-y-4">
                        {bookings.map(booking => (
                            <div key={booking.id} className="bg-gray-50 dark:bg-gray-700 p-4 rounded-lg border border-gray-200 dark:border-gray-600">
                                <p className="text-gray-900 dark:text-white"><span className="font-bold">Event Date:</span> {new Date(booking.eventDate).toLocaleDateString()}</p>
                                <p className="text-gray-900 dark:text-white"><span className="font-bold">Status:</span> {booking.status}</p>
                                <p className="text-gray-900 dark:text-white"><span className="font-bold">Tent Type:</span> {booking.tentType}</p>
                            </div>
                        ))}
                    </div>
                ) : (
                    <p className="text-center text-gray-600 dark:text-gray-400">You have no bookings.</p>
                )}
            </div>
        </div>
    );
}

export default PortalPage;
