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
            const response = await api.post("/api/test/writing-result", {
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
        <div className="max-w-4xl mx-auto p-6 bg-white rounded-lg shadow-md">
            <div className="font-bold text-4xl mb-8 text-blue-700">READING TEST</div>
            {error !== "" && <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-md">{error}</div>}
            <div className="mb-8 p-4 bg-gray-50 rounded-md">
                <h2 className="text-xl font-medium mb-3">Reading Passage:</h2>
                <p className="text-gray-800 leading-relaxed">{testData}</p>
            </div>
            <div className="mb-6">
                <h2 className="text-xl font-medium mb-3">Questions:</h2>
                {questions.map((q, index) => (
                    <div key={index} className="mb-6">
                        <div className="text-lg font-medium mb-2">{`${index + 1}. ${q}`}</div>
                        <input
                            className="w-full p-3 border border-gray-300 rounded-md"
                            placeholder="Enter your answer here"
                            value={answers[index]}
                            onChange={(e) => setAnswers(prev =>
                                prev.map((answer, i) => i === index ? e.target.value : answer)
                            )}
                        />
                    </div>
                ))}
            </div>
            <button
                onClick={submitTest}
                className="mt-6 px-6 py-3 bg-blue-600 text-white font-medium rounded-md hover:bg-blue-700 transition-colors">
                SUBMIT
            </button>
        </div>
    );
}