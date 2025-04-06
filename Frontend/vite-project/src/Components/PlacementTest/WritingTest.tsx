import { useEffect, useState } from "react";
import { api } from "../../axios/api";

export default function WritingTest({ setStage, languageId }: { setStage: React.Dispatch<React.SetStateAction<number>>, languageId: number }) {


    const [testData, setTestData] = useState([]);

    useEffect(() => {
        async function getWritingTest() {
            const response = await api.post("/api/test/writing", {
                languageId: languageId
            });
            if (response.status == 200) setTestData(response.data.questions);
        }
        getWritingTest();
    }, [])


    return (
        <div className="max-w-4xl mx-auto p-6 bg-white rounded-lg shadow-md">
            <div className="font-bold text-4xl mb-8 text-blue-700"> 1, WRITING TEST </div>
            {testData.map((q, index) => (
                <div key={index} className="mb-6">
                    <div className="text-lg font-medium mb-2">{q}</div>
                    <input
                        className="w-full p-3 border border-gray-300 rounded-md"
                        placeholder="Enter your answer here"
                    />
                </div>
            ))}
            <button className="mt-6 px-6 py-3 bg-blue-600 text-white font-medium rounded-md hover:bg-blue-700 transition-colors"> CHECK </button>
        </div>
    );
}
