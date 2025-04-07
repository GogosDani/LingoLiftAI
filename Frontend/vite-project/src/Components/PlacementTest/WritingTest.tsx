import { useEffect, useState } from "react";
import { api } from "../../axios/api";

export default function WritingTest({ setStage, languageId, setLevel }: { setStage: React.Dispatch<React.SetStateAction<number>>, languageId: number, setLevel: React.Dispatch<React.SetStateAction<string>> }) {

    const [error, setError] = useState("");
    const [testData, setTestData] = useState([]);
    const [answers, setAnswers] = useState(["", "", ""]);
    const [questionSetId, setQuestionSetId] = useState(0);

    useEffect(() => {
        async function getWritingTest() {
            try {
                const response = await api.post("/api/test/writing", {
                    languageId: languageId
                });

                if (response.status === 200) {
                    setTestData(response.data.questions);
                    setQuestionSetId(response.data.id);
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
                questionSetId: questionSetId
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
            <div className="font-bold text-4xl mb-8 text-blue-700"> 1, WRITING TEST </div>
            {error == "" && <div> {error} </div>}
            {testData.map((q, index) => (
                <div key={index} className="mb-6">
                    <div className="text-lg font-medium mb-2">{q}</div>
                    <input
                        className="w-full p-3 border border-gray-300 rounded-md"
                        placeholder="Enter your answer here"
                        onChange={(e) => setAnswers(prev =>
                            prev.map((answer, i) => i === index ? e.target.value : answer)
                        )}
                    />
                </div>
            ))}
            <button onClick={() => submitTest()} className="mt-6 px-6 py-3 bg-blue-600 text-white font-medium rounded-md hover:bg-blue-700 transition-colors"> CHECK </button>
        </div>
    );
}
