import { useEffect, useState } from "react"
import { api } from "../../axios/api";
import { useNavigate } from "react-router-dom";

export default function TestResult({ languageId }: { languageId: number }) {

    const [level, setLevel] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        async function getUserLevel() {
            try {
                const response = await api.get(`/api/level/${languageId}`);
                if (response.status == 200) setLevel(response.data.level);
                else {
                    throw new Error(`Error fetching test: ${response.status}`);
                }
            } catch (err) {
                console.error("Failed to fetch writing test:", err);
            }
        }
        getUserLevel();
        setTimeout(() => {
            navigate("/home");
        }, 3000)
    }, [])

    return (
        <div className="max-w-4xl mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-lg">
            <h1 className="font-bold text-3xl sm:text-4xl mb-6 text-blue-700 text-center">Test Results</h1>
            <div className="text-center mb-8">
                <p className="text-xl text-gray-600 mb-2">Your language level is</p>
                <h2 className="font-bold text-4xl mb-3 text-blue-600">
                    {level.toUpperCase()}
                </h2>
                <p className="text-gray-600 max-w-md mx-auto">
                    Based on your test performance, we've assessed your current language abilities.
                </p>
            </div>
        </div>
    );
}