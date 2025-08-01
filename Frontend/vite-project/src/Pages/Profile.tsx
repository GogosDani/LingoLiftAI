import { useEffect, useState } from "react";
import { api } from "../axios/api";
import Headbar from "../Components/Headbar";
import PasswordChangeModal from "../Components/PasswordChangeModal";
import SuccessToast from "../Components/SuccessToast";

interface UserInfo {
    username: string;
    email: string;
}

export default function ProfilePage() {
    const [userInfos, setUserInfos] = useState<UserInfo | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
    const [showSuccessToast, setShowSuccessToast] = useState(false);

    useEffect(() => {
        getUserInfos();
    }, []);

    async function getUserInfos() {
        try {
            const response = await api.get("/api/user");
            const data = await response.data;
            setUserInfos(data);
            console.log(data);
        } catch (error) {
            console.error("Error fetching user info:", error);
        } finally {
            setLoading(false);
        }
    }

    const handlePasswordChangeSuccess = () => {
        setShowSuccessToast(true);
        setTimeout(() => {
            setShowSuccessToast(false);
        }, 3000);
    };


    const handlePasswordReset = (): void => {
        setIsPasswordModalOpen(true);
    };

    const handlePasswordChange = async (currentPassword: string, newPassword: string): Promise<void> => {
        try {
            await api.patch('/api/user', {
                CurrentPassword: currentPassword,
                NewPassword: newPassword
            });
        } catch (error: any) {
            const errorMessage = error.response?.data || error.message || 'Wrong password';
            throw new Error(errorMessage);
        }
    };


    return (
        <div className="min-h-screen bg-gray-50">
            <Headbar />
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">
                        Profile Settings
                    </h1>
                    <p className="text-gray-600 mt-2">Manage your account information</p>
                </div>

                <div className="max-w-2xl mx-auto">
                    <div className="bg-white rounded-xl shadow-lg p-8">
                        <div className="text-center mb-8">
                            <div className="w-32 h-32 bg-gray-200 rounded-full mx-auto flex items-center justify-center">
                                <svg className="w-16 h-16 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
                                    <path fillRule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clipRule="evenodd" />
                                </svg>
                            </div>
                        </div>
                        <div className="space-y-6">
                            <div className="bg-gray-50 rounded-lg p-4">
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Username
                                </label>
                                <p className="text-lg text-gray-800">
                                    {userInfos?.username || "Not available"}
                                </p>
                            </div>

                            <div className="bg-gray-50 rounded-lg p-4">
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Email Address
                                </label>
                                <p className="text-lg text-gray-800">
                                    {userInfos?.email || "Not available"}
                                </p>
                            </div>

                            <div className="pt-4">
                                <button
                                    onClick={handlePasswordReset}
                                    className="w-full bg-blue-500 hover:bg-blue-600 text-white font-medium py-3 px-4 rounded-lg transition-colors duration-300 transform hover:scale-105"
                                >
                                    Reset Password
                                </button>
                            </div>
                            <PasswordChangeModal
                                isOpen={isPasswordModalOpen}
                                onClose={() => setIsPasswordModalOpen(false)}
                                onSubmit={handlePasswordChange}
                                onSuccess={handlePasswordChangeSuccess}
                            />
                            <SuccessToast show={showSuccessToast} />
                            <style dangerouslySetInnerHTML={{
                                __html: `
                @keyframes fade-in {
                    from {
                        opacity: 0;
                        transform: translateY(-10px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }
                
                .animate-fade-in {
                    animation: fade-in 0.3s ease-out;
                }
            `
                            }} />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}