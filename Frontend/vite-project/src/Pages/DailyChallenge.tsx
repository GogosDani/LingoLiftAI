import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Headbar from "../Components/Headbar";
import { api } from "../axios/api";

interface DailyChallenge {
    id: number;
    date: string;
    title: string;
    description: string;
    difficulty: string;
    category: string;
    points: number;
    completed: boolean;
}

export default function DailyChallenge() {
    const navigate = useNavigate();
    const [todaysChallenge, setTodaysChallenge] = useState<DailyChallenge | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function fetchTodaysChallenge() {
            try {
                setLoading(true);
                setError(null);
                const response = await api.get("/api/challenge/today");
                setTodaysChallenge(response.data);
            } catch (error: any) {
                console.error("Error fetching today's challenge:", error);
            } finally {
                setLoading(false);
            }
        }
        fetchTodaysChallenge();
    }, []);



    async function handleStartChallenge() {
        try {
            if (todaysChallenge) {
                navigate(`/challenge/${todaysChallenge.id}/start`);
            }
        } catch (error) {
            console.error("Error starting challenge:", error);
        }
    }




    if (loading) {
        return (
            <div className="min-h-screen bg-gray-50">
                <Headbar />
                <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                    <div className="flex justify-center items-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen bg-gray-50">
                <Headbar />
                <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                    <div className="text-center my-8">
                        <h1 className="text-3xl font-bold text-gray-800">Napi Kihívás</h1>
                    </div>
                    <div className="bg-white rounded-xl shadow-md p-6 mb-8">
                        <div className="text-center text-red-600">
                            <p className="mb-4">{error}</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <Headbar />
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">Daily Challenge</h1>
                </div>

                {todaysChallenge && (
                    <div className="bg-white rounded-xl shadow-md p-6 mb-8">
                        <div className="mb-6">
                            <div className="flex justify-between items-start mb-4">
                                <div>
                                    <h2 className="text-2xl font-bold text-gray-800 mb-2">
                                        {todaysChallenge.title}
                                    </h2>
                                    <div className="flex items-center gap-3 mb-4">
                                        <span className="px-3 py-1 rounded-full text-sm font-medium">
                                            {todaysChallenge.difficulty}
                                        </span>
                                        <span className="text-sm text-gray-500 bg-gray-100 px-3 py-1 rounded-full">
                                            {todaysChallenge.category}
                                        </span>
                                    </div>
                                </div>
                                <div className="text-right">
                                    <p className="text-sm text-gray-500">
                                        {new Date(todaysChallenge.date).toLocaleDateString('hu-HU')}
                                    </p>
                                </div>
                            </div>
                            <div className="flex flex-col md:flex-row gap-4">
                                <button
                                    onClick={handleStartChallenge}
                                    className={`flex-1 p-4 rounded-lg border-2 transition-all ${todaysChallenge.completed
                                        ? "border-gray-300 bg-gray-100 text-gray-500 cursor-not-allowed"
                                        : "border-blue-500 bg-blue-50 hover:bg-blue-100 text-blue-700"
                                        }`}
                                >
                                    <div className="text-center">
                                        <h3 className="font-bold text-lg">
                                            {todaysChallenge.completed ? "Completed" : "Start Challenge"}
                                        </h3>
                                    </div>
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}