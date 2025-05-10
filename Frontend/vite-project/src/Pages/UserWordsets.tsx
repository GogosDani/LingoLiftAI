import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api } from '../axios/api';

interface Wordset {
    id: number;
    name: string;
    firstLanguage: string;
    secondLanguage: string;
    wordCount?: number;
}

export default function UserWordsets() {
    const [wordsets, setWordsets] = useState<Wordset[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const { userId } = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        async function getWordsets() {
            try {
                setIsLoading(true);
                const response = await api.get("/api/wordset");
                if (response.status != 200) {
                    throw new Error('Failed to fetch wordsets');
                }
                const data: Wordset[] = await response.data;
                setWordsets(data);
                setIsLoading(false);
                console.log(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'An unknown error occurred');
            }
        };
        getWordsets();
    }, [userId]);

    function handleCreateNew() {
        navigate('/wordset/custom');
    };

    function handleViewWordset(wordsetId: number) {
        navigate(`/wordset/${wordsetId}`);
    };

    async function handleDelete(wordsetId: number) {
        try {
            const response = await api.delete(`/api/wordset/${wordsetId}`)
            if (response.status != 200) {
                throw new Error('Failed to delete wordset');
            }
            setWordsets(prevWordsets => prevWordsets.filter(wordset => wordset.id !== wordsetId));
        }
        catch (error) {
            console.error(error);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="pt-16 px-4 md:px-12 max-w-7xl mx-auto">
                <div className="text-center my-8">
                    <h1 className="text-3xl font-bold text-gray-800">My Wordsets</h1>
                </div>
                {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4"> {error} </div>
                )}
                <div className="flex justify-end mb-6">
                    <button onClick={handleCreateNew} className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-md flex items-center"> + Create New Wordset
                    </button>
                </div>
                {isLoading ? (
                    <div className="text-center py-12">
                        <p className="text-gray-600">Loading wordsets</p>
                    </div>
                ) : wordsets.length === 0 ? (
                    <div className="bg-white rounded-xl shadow-md p-10 mb-8 text-center">
                        <p className="text-gray-600 text-lg">You don't have any wordsets yet.</p>
                        <button onClick={handleCreateNew} className="mt-4 bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-md"> Create Your First Wordset </button>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                        {wordsets.map((wordset) => (
                            <div key={wordset.id} className="bg-white rounded-xl shadow-md hover:shadow-lg transition-shadow p-6 cursor-pointer relative" onClick={() => handleViewWordset(wordset.id)}>
                                <h2 className="text-xl font-semibold text-gray-800 mb-2 pr-8">{wordset.name}</h2>
                                <div className="flex items-center mb-4">
                                    <div className="ml-2 text-gray-500 text-sm"> {wordset.wordCount || 0} words </div>
                                </div>
                                <button onClick={() => handleDelete(wordset.id)} className="absolute top-4 right-4 text-gray-400 hover:text-red-500 transition-colors bg-transparent p-1 rounded-full hover:bg-red-50"> X</button>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}