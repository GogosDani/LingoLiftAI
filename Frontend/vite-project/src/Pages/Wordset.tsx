import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Headbar from "../Components/Headbar";
import { api } from "../axios/api";


export default function Wordset() {
    const navigate = useNavigate();
    const [wordsetId, setWordsetId] = useState(0);

    async function handleCreateOwnWordset() {
        try {
            navigate("/wordset/custom");
        }
        catch (error) {
            console.error(error);
        }
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <Headbar />
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">WordSets</h1>
                    <p className="text-gray-600 mt-2"> Learn new vocabulary with customized word sets </p>
                </div>
                <div className="bg-white rounded-xl shadow-md p-6 mb-8">
                    <div className="mb-6">
                        <h2 className="text-xl font-bold text-gray-800 mb-4">  How would you like to learn?  </h2>
                        <div className="flex flex-col md:flex-row gap-4">
                            <button className={`flex-1 p-4 rounded-lg border-2 transition-all ? "border-blue-500 bg-blue-50" : "border-gray-200 hover:border-blue-300" }`}>
                                <div className="flex items-center">
                                    <div className="text-left">
                                        <h3 className="font-bold text-lg">AI-Generated WordSet</h3>
                                        <p className="text-gray-600 text-sm"> Get vocabulary customized to your selected topic </p>
                                    </div>
                                </div>
                            </button>
                            <button onClick={() => handleCreateOwnWordset()} className={`flex-1 p-4 rounded-lg border-2 transition-all ? "border-green-500 bg-green-50": "border-gray-200 hover:border-green-300"}`} >
                                <div className="flex items-center">
                                    <div className="text-left">
                                        <h3 className="font-bold text-lg">Create Your Own</h3>
                                        <p className="text-gray-600 text-sm"> Build a custom wordset with your own words and definitions  </p>
                                    </div>
                                </div>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}