import { useState, useEffect } from 'react';
import { DndProvider } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';
import TextWithEmptyPlaces from '../DragAndDrop/TextWithEmptyPlaces';
import DraggableWord from '../DragAndDrop/DraggableWord';
import { api } from '../../axios/api';

export default function BlindedTest({ setStage, languageId }: { setStage: React.Dispatch<React.SetStateAction<number>>, languageId: number }) {
    const [testData, setTestData] = useState("");
    const [words, setWords] = useState<string[]>([]);
    const [error, setError] = useState("");
    const [filledSpaces, setFilledSpaces] = useState<{ [key: number]: string }>({});
    const [testId, setTestId] = useState(0);
    const [level, setLevel] = useState("");

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
        async function getBlindedTest() {
            try {
                const response = await api.post("/api/test/blinded", {
                    languageId: languageId
                });
                if (response.status === 200) {
                    setTestData(response.data.story);
                    setWords(response.data.words);
                    setTestId(response.data.id);
                } else {
                    throw new Error(`Error fetching test: ${response.status}`);
                }
            } catch (err) {
                console.error("Failed to fetch writing test:", err);
            }
        }
        getBlindedTest();
    }, [languageId]);

    const handleDrop = (index: number, word: string) => {
        setFilledSpaces(prev => ({
            ...prev,
            [index]: word
        }));

        setWords(prev => prev.filter(w => w !== word));
    };

    const handleRemoveWord = (index: number) => {
        const word = filledSpaces[index];
        if (word) {
            const newFilledSpaces = { ...filledSpaces };
            delete newFilledSpaces[index];
            setFilledSpaces(newFilledSpaces);
            setWords(prev => [...prev, word]);
        }
    };

    async function submitTest() {
        try {
            const response = await api.post("/api/test/blinded-result", {
                languageId: languageId,
                userAnswers: Object.values(filledSpaces),
                blindedTestId: testId
            });
            setStage(prev => prev + 1);
            console.log(response.data);
        }
        catch (error) {
            console.error("Failed to submit writing test: " + error);
            setError("Unable to submit your answers. Please try again.");
        }
    }

    return (
        <DndProvider backend={HTML5Backend}>
            <div className="max-w-4xl mx-auto p-4 sm:p-6 bg-white rounded-lg shadow-lg">
                <h1 className="font-bold text-3xl sm:text-4xl mb-6 text-blue-700 text-center">Fill in the Blanks</h1>
                <div className="mb-8 p-4 bg-gray-50 rounded-md border border-gray-100 shadow-sm">
                    <TextWithEmptyPlaces
                        text={testData}
                        filledSpaces={filledSpaces}
                        onRemoveWord={handleRemoveWord}
                        onDrop={handleDrop}
                    />
                </div>
                <div className="mt-8">
                    <h2 className="text-xl font-semibold mb-4 text-gray-800">Available Words:</h2>
                    <div className="flex flex-wrap gap-3 justify-center sm:justify-start">
                        {words.map((word, index) => (
                            <DraggableWord key={`${word}-${index}`} word={word} />
                        ))}
                    </div>
                </div>
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
        </DndProvider>
    );
}