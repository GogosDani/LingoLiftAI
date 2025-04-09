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
        <>
            <div> {testData} </div>
            <div>
                {words.map(w => <div> {w} </div>)}
            </div>
        </>
    )
}