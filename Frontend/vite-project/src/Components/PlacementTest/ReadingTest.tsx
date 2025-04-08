import { useEffect, useState } from "react";
import { api } from "../../axios/api";

export default function ReadingTest({ setStage, languageId, level, setLevel }: { setStage: React.Dispatch<React.SetStateAction<number>>, languageId: number, level: string, setLevel: React.Dispatch<React.SetStateAction<string>> }) {


    const [testId, setTestId] = useState(0);
    const [testData, setTestData] = useState("");
    const [questions, SetQuestions] = useState([]);
    const [answers, setAnswers] = useState(["", "", ""]);
    const [error, setError] = useState("");


    useEffect(() => {
        async function getWritingTest() {
            try {
                const response = await api.post("/api/test/reading", {
                    languageId: languageId
                });
                if (response.status === 200) {
                    setTestData(response.data.text);
                    setTestId(response.data.id);
                    SetQuestions(response.data.questions);
                } else {
                    throw new Error(`Error fetching test: ${response.status}`);
                }
            } catch (err) {
                console.error("Failed to fetch writing test:", err);
            }
        }

        getWritingTest();
    }, []);

    async function submitTest() {
        if (answers.includes("")) {
            setError("You must answers every question!");
            return;
        }
        try {
            const response = await api.post("/api/test/reading-result", {
                languageId: languageId,
                answers: answers,
                readingTestId: testId
            });
            setLevel(response.data);
            setStage(prev => prev + 1);
        }
        catch (error) {
            console.error("Failed to submit writing test: " + error);
            setError("Unable to submit your answers. Please try again.");
        }
    }
    return (
        <div className="max-w-4xl mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-lg">
            <h1 className="font-bold text-3xl sm:text-4xl mb-6 sm:mb-8 text-blue-700 text-center sm:text-left">READING TEST</h1>
            {error !== "" && (
                <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-md border border-red-200 font-medium">
                    {error}
                </div>
            )}
            <div className="mb-6 sm:mb-8 p-4 bg-gray-50 rounded-md border border-gray-100 shadow-sm">
                <h2 className="text-xl font-semibold mb-3 text-gray-800">Reading Passage:</h2>
                <p className="text-gray-800 leading-relaxed">{testData}</p>
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-semibold mb-4 text-gray-800">Questions:</h2>
                {questions.map((q, index) => (
                    <div key={index} className="mb-8">
                        <div className="text-lg font-medium mb-2 text-gray-700">{`${index + 1}. ${q}`}</div>
                        <input
                            className="w-full p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 focus:outline-none transition-all"
                            placeholder="Enter your answer here"
                            value={answers[index]}
                            onChange={(e) => setAnswers(prev =>
                                prev.map((answer, i) => i === index ? e.target.value : answer)
                            )}
                        />
                    </div>
                ))}
            </div>
            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                <button
                    onClick={submitTest}
                    className="w-full sm:w-auto mb-4 sm:mb-0 px-6 py-3 bg-blue-600 text-white font-medium rounded-md hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 transition-all shadow-sm">
                    SUBMIT
                </button>
                <div className="font-bold text-xl sm:text-2xl text-gray-800">
                    Current Level: <span className="text-blue-600">{level}</span>
                </div>
            </div>
        </div>
    );
}