import { useEffect } from "react"
import { api } from "../axios/api";
import { useNavigate } from "react-router-dom";
import Headbar from "../Components/Headbar";

export default function Home() {

    const navigate = useNavigate();

    useEffect(() => {
        async function getUserLanguage() {
            const response = await api.get("/api/test/check-test-status");
            if (!response.data.testAvailable) navigate("/placement-test");
        }
        getUserLanguage();
    }, [])

    return (
        <div className="min-h-screen bg-gray-50">
            <Headbar />
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">
                        Welcome back, USER
                    </h1>
                    <p className="text-gray-600 mt-2">What would you like to learn today?</p>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 my-8">
                    <div onClick={() => navigate("/daily-challenge")} className="bg-green-500 hover:bg-green-600 rounded-xl p-6 text-white shadow-lg cursor-pointer transform transition-transform duration-300 hover:scale-105">
                        <div>
                            <h2 className="text-xl font-bold">Daily Challenge</h2>
                            <p className="text-sm opacity-90">Complete daily exercises to maintain your streak</p>
                        </div>
                    </div>
                    <div onClick={() => navigate("/wordset")} className="bg-blue-500 hover:bg-blue-600 rounded-xl p-6 text-white shadow-lg cursor-pointer transform transition-transform duration-300 hover:scale-105">
                        <div>
                            <h2 className="text-xl font-bold">Flashcards</h2>
                            <p className="text-sm opacity-90">Review vocabulary with interactive flashcards</p>
                        </div>
                    </div>
                    <div className="bg-purple-500 hover:bg-purple-600 rounded-xl p-6 text-white shadow-lg cursor-pointer transform transition-transform duration-300 hover:scale-105">
                        <div>
                            <h2 className="text-xl font-bold">Learn with LingoAI</h2>
                            <p className="text-sm opacity-90">Interactive lessons with AI</p>
                        </div>
                    </div>
                    <div className="bg-orange-500 hover:bg-orange-600 rounded-xl p-6 text-white shadow-lg cursor-pointer transform transition-transform duration-300 hover:scale-105">
                        <div>
                            <h2 className="text-xl font-bold">Play Against Others</h2>
                            <p className="text-sm opacity-90">Test your skills in multiplayer games</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}