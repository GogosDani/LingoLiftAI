import { useEffect, useState } from "react";
import { api } from "../../axios/api";

export default function CorrectTest({ languageId, setStage }: { languageId: number, setStage: React.Dispatch<React.SetStateAction<number>> }) {

    const [sentences, setSentences] = useState([]);
    const [testId, setTestId] = useState(0);
    const [level, setLevel] = useState("");
    const [corrections, setCorrections] = useState(["", "", "", "", ""])

    useEffect(() => {
        async function getLevel() {
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
        getLevel();
    }, [])

    useEffect(() => {
        async function getCorrectionTest() {
            try {
                const response = await api.post("/api/test/correction", {
                    languageId: languageId
                });
                if (response.status === 200) {
                    setSentences(response.data.sentences);
                    setTestId(response.data.id);
                } else {
                    throw new Error(`Error fetching test: ${response.status}`);
                }
            } catch (err) {
                console.error("Failed to fetch writing test:", err);
            }
        }
        getCorrectionTest();
    }, []);

    async function handleSubmit() {
        try {
            const response = await api.post("/api/test/correction-result", {
                languageId: languageId,
                correctionId: testId,
                answers: corrections
            });
            setStage(prev => prev + 1);
            console.log(response.data);
        }
        catch (error) {
            console.error("Failed to submit writing test: " + error);
        }
    }

    return (
        <div className="max-w-4xl mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-lg">
            <h1 className="font-bold text-3xl sm:text-4xl mb-6 text-blue-700 text-center">Sentence Correction</h1>
            <div className="mb-8 space-y-6">
                {sentences.map((sentence, index) => (
                    <div key={index} className="p-4 bg-gray-50 rounded-md border border-gray-100 shadow-sm">
                        <div className="mb-3 text-lg font-medium text-gray-800">{sentence}</div>
                        <input
                            onChange={(e) => setCorrections(prev =>
                                prev.map((answer, i) => i === index ? e.target.value : answer)
                            )}
                            type="text"
                            placeholder="Enter your correction here"
                            className="w-full px-4 py-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                        />
                    </div>
                ))}
            </div>

            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                <button
                    onClick={() => handleSubmit()}
                    className="w-full sm:w-auto mb-4 sm:mb-0 px-6 py-3 bg-blue-600 text-white font-medium rounded-md hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 transition-all shadow-sm">
                    SUBMIT
                </button>
                <div className="font-bold text-xl sm:text-2xl text-gray-800">
                    Current Level: <span className="text-blue-600">{level}</span>
                </div>
            </div>
        </div>
    );
};
