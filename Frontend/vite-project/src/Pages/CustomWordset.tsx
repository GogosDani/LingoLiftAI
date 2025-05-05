import { useState, useEffect } from "react";
import { api } from "../axios/api";

export default function CustomWordset() {
    interface Language {
        id: number;
        languageName: string;
        flag: string;
    }

    interface WordPair {
        id: number;
        first: string;
        second: string;
        isTemp?: boolean;
    }

    const [wordsetName, setWordsetName] = useState<string>("wordset");
    const [firstLanguageId, setFirstLanguageId] = useState<number>(1);
    const [secondLanguageId, setSecondLanguageId] = useState<number>(2);
    const [wordPairs, setWordPairs] = useState<WordPair[]>([{ id: 1, first: "", second: "", isTemp: true }]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>("");
    const [languages, setLanguages] = useState<Language[]>([]);
    const [wordsetId, setWordsetId] = useState<number>();

    useEffect(() => {
        async function getLanguages() {
            const response = await api.get("/api/language");
            if (response.status == 200) setLanguages(response.data);
            console.log(response.data);
        }
        getLanguages();
        createWordset();
    }, [])

    async function createWordset() {
        const response = await api.post("/api/wordset", wordsetName, {
            params: {
                firstLanguageId: firstLanguageId,
                secondLanguageId: secondLanguageId
            },
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const id = await response.data;
        setWordsetId(id);
    }

    function handleAddWordPair() {
        const tempId = Math.max(...wordPairs.map(pair => pair.id), 0) + 1;
        const newPair: WordPair = { id: tempId, first: "", second: "", isTemp: true };
        setWordPairs([...wordPairs, newPair]);
    }

    function handleWordPairChange(id: number, field: 'first' | 'second', value: string) {
        const updatedPairs = wordPairs.map(pair => {
            if (pair.id === id) {
                return { ...pair, [field]: value };
            }
            return pair;
        });
        setWordPairs(updatedPairs);
    }

    async function handleRemoveWordPair(id: number) {
        const pairToDelete = wordPairs.find(pair => pair.id === id);
        if (pairToDelete && !pairToDelete.isTemp) {
            try {
                const response = await api.delete(`/api/wordset/wordpair/${id}`);
                if (response.status != 200) {
                    setError("Failed to delete word pair");
                    return;
                }
            } catch (error) {
                if (error instanceof Error) {
                    setError("Failed to delete word pair: " + error.message);
                } else {
                    setError("Failed to delete word pair: unknown error");
                }
                return;
            }
        }
        setWordPairs(wordPairs.filter(pair => pair.id !== id));
    }

    async function handleSaveWordset() {
        setIsLoading(true);
        setError("");
        try {
            for (const pair of wordPairs) {
                if (pair.first.trim() === "" && pair.second.trim() === "") {
                    continue;
                }

                if (pair.isTemp) {
                    const pairResponse = await api.post(`/api/wordset/${wordsetId}/wordpair`);
                    const newPairId = pairResponse.data;
                    await api.put(`/api/wordset/wordpair/${newPairId}`, {
                        firstWord: pair.first,
                        secondWord: pair.second
                    });
                } else {
                    await api.put(`/api/wordset/wordpair/${pair.id}`, {
                        firstWord: pair.first,
                        secondWord: pair.second
                    });
                }
            }
            console.log("save")
        } catch (error) {
            setError("Failed to save wordset: unknown error");
        }
    }

    function handleCancel() {
        setWordsetName("");
        setWordPairs([{ id: 1, first: "", second: "", isTemp: true }]);
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">Create New Wordset</h1>
                </div>
                {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                        {error}
                    </div>
                )}
                <div className="bg-white rounded-xl shadow-md p-6 mb-8">
                    <div className="mb-6">
                        <label htmlFor="wordsetName" className="block text-gray-700 font-medium mb-2"> Wordset Name </label>
                        <input id="wordsetName" value={wordsetName} onChange={(e) => setWordsetName(e.target.value)} className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Enter wordset name" />
                    </div>
                    <div className="flex flex-col md:flex-row gap-4 mb-6">
                        <div className="flex-1">
                            <label htmlFor="sourceLang" className="block text-gray-700 font-medium mb-2"> First Language </label>
                            <select id="sourceLang" value={firstLanguageId} onChange={(e) => setFirstLanguageId(parseInt(e.target.value))} className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" >
                                {languages.map(lang => (
                                    <option key={lang.id} value={lang.id}>
                                        {lang.languageName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="flex-1">
                            <label htmlFor="targetLang" className="block text-gray-700 font-medium mb-2"> Second Language </label>
                            <select id="targetLang" value={secondLanguageId} onChange={(e) => setSecondLanguageId(parseInt(e.target.value))} className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" >
                                {languages.map(lang => (
                                    <option key={lang.id} value={lang.id}>
                                        {lang.languageName}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                    <div className="mb-6">
                        <div className="flex justify-between mb-4">
                            <h2 className="text-xl font-semibold text-gray-800">Word Pairs</h2>
                        </div>
                        <div className="space-y-4">
                            {wordPairs.map((pair) => (
                                <div key={pair.id} className="flex flex-col md:flex-row gap-4">
                                    <div className="flex-1">
                                        <input
                                            value={pair.first}
                                            onChange={(e) => handleWordPairChange(pair.id, "first", e.target.value)}
                                            className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            placeholder="First word"
                                        />
                                    </div>

                                    <div className="flex-1">
                                        <input
                                            value={pair.second}
                                            onChange={(e) => handleWordPairChange(pair.id, "second", e.target.value)}
                                            className="w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            placeholder="Second word"
                                        />
                                    </div>

                                    <button
                                        onClick={() => handleRemoveWordPair(pair.id)}
                                        className="bg-red-500 hover:bg-red-600 text-white px-3 py-2 rounded-md"
                                    >
                                        Remove
                                    </button>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className="flex justify-start gap-4">
                        <button onClick={handleAddWordPair} className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-md flex items-center" >
                            + Add Word
                        </button>
                    </div>
                    <div className="flex justify-end gap-4 mt-6">
                        <button onClick={handleCancel} className="px-6 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-100" disabled={isLoading}>
                            Reset
                        </button>
                        <button onClick={handleSaveWordset} className="px-6 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-md" disabled={isLoading}>
                            {isLoading ? "Saving..." : "Save Wordset"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}