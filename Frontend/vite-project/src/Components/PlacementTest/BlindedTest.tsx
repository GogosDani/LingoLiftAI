import { useState, useEffect } from "react";
import { api } from "../../axios/api";

export default function BlindedTest({ setStage, languageId }: { setStage: React.Dispatch<React.SetStateAction<number>>, languageId: number }) {

    const [testId, setTestId] = useState(0);
    const [testData, setTestData] = useState("");
    const [words, setWords] = useState([]);
    const [answers, setAnswers] = useState(["", "", ""]);
    const [error, setError] = useState("");

    // TODO: Get user's level from BE

    useEffect(() => {
        async function getBlindedTest() {
            try {
                const response = await api.post("/api/test/blinded", {
                    languageId: languageId
                });
                if (response.status === 200) {
                    setTestData(response.data.story);
                    setWords(response.data.words)
                } else {
                    throw new Error(`Error fetching test: ${response.status}`);
                }
            } catch (err) {
                console.error("Failed to fetch writing test:", err);
            }
        }
        getBlindedTest();
    }, []);


    return (
        <div className="max-w-4xl mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-lg">
            <h1 className="font-bold text-3xl sm:text-4xl mb-6 text-blue-700 text-center">Fill in the Blanks</h1>
            <div className="mb-8 p-4 bg-gray-50 rounded-md border border-gray-100 shadow-sm">
                <p className="text-gray-800 leading-relaxed text-lg">
                    {testData}
                </p>
            </div>
            <div className="mt-8">
                <h2 className="text-xl font-semibold mb-4 text-gray-800">Available Words:</h2>
                <div className="flex flex-wrap gap-3 justify-center sm:justify-start">
                    {words.map((w, index) => (
                        <div key={index} className="px-4 py-3 bg-blue-50 text-blue-700 rounded-md border-2 border-blue-200 shadow-sm cursor-move hover:shadow-md transition-all font-medium text-center min-w-20">
                            {w}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}